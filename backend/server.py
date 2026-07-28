from dotenv import load_dotenv
from pathlib import Path

ROOT_DIR = Path(__file__).parent
load_dotenv(ROOT_DIR / '.env')

import os
import uuid
import logging
from datetime import datetime, timezone, timedelta
from typing import List, Optional, Any

import bcrypt
import jwt
from fastapi import FastAPI, APIRouter, HTTPException, Depends, Request, Response
from starlette.middleware.cors import CORSMiddleware
from motor.motor_asyncio import AsyncIOMotorClient
from pydantic import BaseModel, Field, EmailStr, ConfigDict

# ------------------- Env & DB -------------------
mongo_url = os.environ['MONGO_URL']
client = AsyncIOMotorClient(mongo_url)
db = client[os.environ['DB_NAME']]

JWT_SECRET = os.environ['JWT_SECRET']
JWT_ALGORITHM = "HS256"
ACCESS_TOKEN_EXPIRE_MINUTES = 60 * 24 * 7  # 7 days for web app usability
FRONTEND_URL = os.environ.get('FRONTEND_URL', 'http://localhost:3000')

app = FastAPI(title="RideAtlas API")
api = APIRouter(prefix="/api")

logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(name)s - %(levelname)s - %(message)s')
logger = logging.getLogger(__name__)


# ------------------- Helpers -------------------
def now_iso() -> str:
    return datetime.now(timezone.utc).isoformat()

def hash_password(password: str) -> str:
    return bcrypt.hashpw(password.encode("utf-8"), bcrypt.gensalt()).decode("utf-8")

def verify_password(plain: str, hashed: str) -> bool:
    return bcrypt.checkpw(plain.encode("utf-8"), hashed.encode("utf-8"))

def create_access_token(user_id: str, email: str) -> str:
    payload = {
        "sub": user_id,
        "email": email,
        "type": "access",
        "exp": datetime.now(timezone.utc) + timedelta(minutes=ACCESS_TOKEN_EXPIRE_MINUTES),
    }
    return jwt.encode(payload, JWT_SECRET, algorithm=JWT_ALGORITHM)

async def get_current_user(request: Request) -> dict:
    token = request.cookies.get("access_token")
    if not token:
        auth_header = request.headers.get("Authorization", "")
        if auth_header.startswith("Bearer "):
            token = auth_header[7:]
    if not token:
        raise HTTPException(status_code=401, detail="Not authenticated")
    try:
        payload = jwt.decode(token, JWT_SECRET, algorithms=[JWT_ALGORITHM])
        if payload.get("type") != "access":
            raise HTTPException(status_code=401, detail="Invalid token type")
        user = await db.users.find_one({"id": payload["sub"]}, {"password_hash": 0, "_id": 0})
        if not user:
            raise HTTPException(status_code=401, detail="User not found")
        return user
    except jwt.ExpiredSignatureError:
        raise HTTPException(status_code=401, detail="Token expired")
    except jwt.InvalidTokenError:
        raise HTTPException(status_code=401, detail="Invalid token")


# ------------------- Models -------------------
class UserRegister(BaseModel):
    email: EmailStr
    password: str = Field(min_length=6)
    name: str = Field(min_length=1, max_length=80)

class UserLogin(BaseModel):
    email: EmailStr
    password: str

class Bike(BaseModel):
    model_config = ConfigDict(extra="ignore")
    id: str = Field(default_factory=lambda: str(uuid.uuid4()))
    user_id: str = ""
    name: str
    make: str
    model: str
    year: int
    cc: int
    reg_no: Optional[str] = ""
    odometer: int = 0
    fuel_type: str = "Petrol"
    avg_mileage: float = 0.0
    insurance_valid_until: Optional[str] = None
    puc_valid_until: Optional[str] = None
    is_primary: bool = False
    image_url: Optional[str] = None
    created_at: str = Field(default_factory=now_iso)

class BikeCreate(BaseModel):
    name: str
    make: str
    model: str
    year: int
    cc: int
    reg_no: Optional[str] = ""
    odometer: int = 0
    fuel_type: str = "Petrol"
    avg_mileage: float = 0.0
    insurance_valid_until: Optional[str] = None
    puc_valid_until: Optional[str] = None
    is_primary: bool = False
    image_url: Optional[str] = None

class Waypoint(BaseModel):
    name: str
    lat: float
    lng: float
    note: Optional[str] = ""

class TripDay(BaseModel):
    day: int
    from_city: str
    to_city: str
    distance_km: int
    weather_temp: Optional[int] = None
    fuel_stop: Optional[str] = ""
    overnight: Optional[str] = ""

class Trip(BaseModel):
    model_config = ConfigDict(extra="ignore")
    id: str = Field(default_factory=lambda: str(uuid.uuid4()))
    user_id: str = ""
    title: str
    start_date: str
    end_date: str
    budget: float = 0
    riders: int = 1
    distance_km: int = 0
    waypoints: List[Waypoint] = []
    days: List[TripDay] = []
    created_at: str = Field(default_factory=now_iso)

class TripCreate(BaseModel):
    title: str
    start_date: str
    end_date: str
    budget: float = 0
    riders: int = 1
    distance_km: int = 0
    waypoints: List[Waypoint] = []
    days: List[TripDay] = []

class FuelEntry(BaseModel):
    model_config = ConfigDict(extra="ignore")
    id: str = Field(default_factory=lambda: str(uuid.uuid4()))
    user_id: str = ""
    bike_id: Optional[str] = None
    date: str
    odometer: int
    liters: float
    cost: float
    station: str = ""
    fuel_type: str = "Petrol"
    computed_mileage: Optional[float] = None
    created_at: str = Field(default_factory=now_iso)

class FuelEntryCreate(BaseModel):
    bike_id: Optional[str] = None
    date: str
    odometer: int
    liters: float
    cost: float
    station: str = ""
    fuel_type: str = "Petrol"

class ServiceRecord(BaseModel):
    model_config = ConfigDict(extra="ignore")
    id: str = Field(default_factory=lambda: str(uuid.uuid4()))
    user_id: str = ""
    bike_id: Optional[str] = None
    date: str
    odometer: int
    description: str
    center: str = ""
    cost: float = 0
    notes: Optional[str] = ""
    created_at: str = Field(default_factory=now_iso)

class ServiceCreate(BaseModel):
    bike_id: Optional[str] = None
    date: str
    odometer: int
    description: str
    center: str = ""
    cost: float = 0
    notes: Optional[str] = ""

class Expense(BaseModel):
    model_config = ConfigDict(extra="ignore")
    id: str = Field(default_factory=lambda: str(uuid.uuid4()))
    user_id: str = ""
    trip_id: Optional[str] = None
    date: str
    category: str  # Fuel, Hotel, Food, Toll, Other
    amount: float
    note: Optional[str] = ""
    split_riders: int = 1
    created_at: str = Field(default_factory=now_iso)

class ExpenseCreate(BaseModel):
    trip_id: Optional[str] = None
    date: str
    category: str
    amount: float
    note: Optional[str] = ""
    split_riders: int = 1

class JournalEntry(BaseModel):
    model_config = ConfigDict(extra="ignore")
    id: str = Field(default_factory=lambda: str(uuid.uuid4()))
    user_id: str = ""
    trip_id: Optional[str] = None
    date: str
    title: str
    location: str = ""
    weather: str = ""
    companions: str = ""
    mood: str = ""
    distance_km: int = 0
    duration_min: int = 0
    notes: str = ""
    photos: List[str] = []
    ai_generated: bool = False
    created_at: str = Field(default_factory=now_iso)

class JournalCreate(BaseModel):
    trip_id: Optional[str] = None
    date: str
    title: str
    location: str = ""
    weather: str = ""
    companions: str = ""
    mood: str = ""
    distance_km: int = 0
    duration_min: int = 0
    notes: str = ""
    photos: List[str] = []

class PackItem(BaseModel):
    model_config = ConfigDict(extra="ignore")
    id: str = Field(default_factory=lambda: str(uuid.uuid4()))
    user_id: str = ""
    trip_id: Optional[str] = None
    category: str  # Riding gear, Documents, Electronics, Other
    name: str
    packed: bool = False
    created_at: str = Field(default_factory=now_iso)

class PackItemCreate(BaseModel):
    trip_id: Optional[str] = None
    category: str
    name: str
    packed: bool = False


# ------------------- Startup -------------------
DEMO_EMAIL = "demo@rideatlas.com"
DEMO_PASSWORD = "demo1234"

@app.on_event("startup")
async def startup():
    await db.users.create_index("email", unique=True)
    await db.bikes.create_index("user_id")
    await db.trips.create_index("user_id")
    await db.fuel_entries.create_index("user_id")
    await db.service_records.create_index("user_id")
    await db.expenses.create_index("user_id")
    await db.journal_entries.create_index("user_id")
    await db.pack_items.create_index("user_id")
    # Seed demo user (idempotent)
    existing = await db.users.find_one({"email": DEMO_EMAIL})
    if not existing:
        await db.users.insert_one({
            "id": str(uuid.uuid4()),
            "email": DEMO_EMAIL,
            "name": "Demo Rider",
            "password_hash": hash_password(DEMO_PASSWORD),
            "created_at": now_iso(),
        })
        logger.info("Seeded demo user %s", DEMO_EMAIL)


# ------------------- Auth Routes -------------------
def set_auth_cookie(response: Response, token: str):
    response.set_cookie(
        key="access_token",
        value=token,
        httponly=True,
        secure=True,
        samesite="none",
        max_age=ACCESS_TOKEN_EXPIRE_MINUTES * 60,
        path="/",
    )

@api.post("/auth/register")
async def register(payload: UserRegister, response: Response):
    email = payload.email.lower()
    existing = await db.users.find_one({"email": email})
    if existing:
        raise HTTPException(status_code=400, detail="Email already registered")
    user_id = str(uuid.uuid4())
    doc = {
        "id": user_id,
        "email": email,
        "name": payload.name,
        "password_hash": hash_password(payload.password),
        "created_at": now_iso(),
    }
    await db.users.insert_one(doc)
    token = create_access_token(user_id, email)
    set_auth_cookie(response, token)
    return {"id": user_id, "email": email, "name": payload.name, "token": token}

@api.post("/auth/login")
async def login(payload: UserLogin, response: Response):
    email = payload.email.lower()
    user = await db.users.find_one({"email": email})
    if not user or not verify_password(payload.password, user["password_hash"]):
        raise HTTPException(status_code=401, detail="Invalid email or password")
    token = create_access_token(user["id"], email)
    set_auth_cookie(response, token)
    return {"id": user["id"], "email": email, "name": user["name"], "token": token}

@api.post("/auth/logout")
async def logout(response: Response):
    response.delete_cookie("access_token", path="/")
    return {"ok": True}

@api.get("/auth/me")
async def me(user: dict = Depends(get_current_user)):
    return {"id": user["id"], "email": user["email"], "name": user["name"]}


# ------------------- Generic per-user helpers -------------------
async def _list(collection, user_id: str) -> List[dict]:
    docs = await collection.find({"user_id": user_id}, {"_id": 0}).sort("created_at", -1).to_list(1000)
    return docs

async def _get(collection, item_id: str, user_id: str) -> dict:
    doc = await collection.find_one({"id": item_id, "user_id": user_id}, {"_id": 0})
    if not doc:
        raise HTTPException(status_code=404, detail="Not found")
    return doc

async def _delete(collection, item_id: str, user_id: str):
    res = await collection.delete_one({"id": item_id, "user_id": user_id})
    if res.deleted_count == 0:
        raise HTTPException(status_code=404, detail="Not found")
    return {"ok": True}


# ------------------- Bikes -------------------
@api.get("/bikes")
async def list_bikes(user: dict = Depends(get_current_user)):
    return await _list(db.bikes, user["id"])

@api.post("/bikes")
async def create_bike(payload: BikeCreate, user: dict = Depends(get_current_user)):
    bike = Bike(**payload.model_dump(), user_id=user["id"])
    doc = bike.model_dump()
    if bike.is_primary:
        await db.bikes.update_many({"user_id": user["id"]}, {"$set": {"is_primary": False}})
    await db.bikes.insert_one(doc)
    doc.pop("_id", None)
    return doc

@api.put("/bikes/{bike_id}")
async def update_bike(bike_id: str, payload: BikeCreate, user: dict = Depends(get_current_user)):
    existing = await _get(db.bikes, bike_id, user["id"])
    updates = payload.model_dump()
    if updates.get("is_primary"):
        await db.bikes.update_many({"user_id": user["id"]}, {"$set": {"is_primary": False}})
    await db.bikes.update_one({"id": bike_id, "user_id": user["id"]}, {"$set": updates})
    existing.update(updates)
    return existing

@api.delete("/bikes/{bike_id}")
async def delete_bike(bike_id: str, user: dict = Depends(get_current_user)):
    return await _delete(db.bikes, bike_id, user["id"])


# ------------------- Trips -------------------
@api.get("/trips")
async def list_trips(user: dict = Depends(get_current_user)):
    return await _list(db.trips, user["id"])

@api.post("/trips")
async def create_trip(payload: TripCreate, user: dict = Depends(get_current_user)):
    trip = Trip(**payload.model_dump(), user_id=user["id"])
    doc = trip.model_dump()
    await db.trips.insert_one(doc)
    doc.pop("_id", None)
    return doc

@api.get("/trips/{trip_id}")
async def get_trip(trip_id: str, user: dict = Depends(get_current_user)):
    return await _get(db.trips, trip_id, user["id"])

@api.put("/trips/{trip_id}")
async def update_trip(trip_id: str, payload: TripCreate, user: dict = Depends(get_current_user)):
    existing = await _get(db.trips, trip_id, user["id"])
    updates = payload.model_dump()
    await db.trips.update_one({"id": trip_id, "user_id": user["id"]}, {"$set": updates})
    existing.update(updates)
    return existing

@api.delete("/trips/{trip_id}")
async def delete_trip(trip_id: str, user: dict = Depends(get_current_user)):
    return await _delete(db.trips, trip_id, user["id"])


# ------------------- Fuel -------------------
@api.get("/fuel")
async def list_fuel(user: dict = Depends(get_current_user)):
    docs = await db.fuel_entries.find({"user_id": user["id"]}, {"_id": 0}).sort("date", -1).to_list(1000)
    return docs

@api.post("/fuel")
async def create_fuel(payload: FuelEntryCreate, user: dict = Depends(get_current_user)):
    entry = FuelEntry(**payload.model_dump(), user_id=user["id"])
    # Compute mileage from prior fill-up on same bike
    prev = await db.fuel_entries.find_one(
        {"user_id": user["id"], "bike_id": entry.bike_id, "odometer": {"$lt": entry.odometer}},
        sort=[("odometer", -1)],
        projection={"_id": 0},
    )
    if prev and entry.liters > 0:
        km = entry.odometer - prev["odometer"]
        if km > 0:
            entry.computed_mileage = round(km / entry.liters, 2)
    doc = entry.model_dump()
    await db.fuel_entries.insert_one(doc)
    doc.pop("_id", None)
    return doc

@api.delete("/fuel/{entry_id}")
async def delete_fuel(entry_id: str, user: dict = Depends(get_current_user)):
    return await _delete(db.fuel_entries, entry_id, user["id"])


# ------------------- Service -------------------
@api.get("/service")
async def list_service(user: dict = Depends(get_current_user)):
    docs = await db.service_records.find({"user_id": user["id"]}, {"_id": 0}).sort("date", -1).to_list(1000)
    return docs

@api.post("/service")
async def create_service(payload: ServiceCreate, user: dict = Depends(get_current_user)):
    rec = ServiceRecord(**payload.model_dump(), user_id=user["id"])
    doc = rec.model_dump()
    await db.service_records.insert_one(doc)
    doc.pop("_id", None)
    return doc

@api.delete("/service/{service_id}")
async def delete_service(service_id: str, user: dict = Depends(get_current_user)):
    return await _delete(db.service_records, service_id, user["id"])


# ------------------- Expenses -------------------
@api.get("/expenses")
async def list_expenses(user: dict = Depends(get_current_user)):
    docs = await db.expenses.find({"user_id": user["id"]}, {"_id": 0}).sort("date", -1).to_list(1000)
    return docs

@api.post("/expenses")
async def create_expense(payload: ExpenseCreate, user: dict = Depends(get_current_user)):
    exp = Expense(**payload.model_dump(), user_id=user["id"])
    doc = exp.model_dump()
    await db.expenses.insert_one(doc)
    doc.pop("_id", None)
    return doc

@api.delete("/expenses/{expense_id}")
async def delete_expense(expense_id: str, user: dict = Depends(get_current_user)):
    return await _delete(db.expenses, expense_id, user["id"])


# ------------------- Journal -------------------
@api.get("/journal")
async def list_journal(user: dict = Depends(get_current_user)):
    docs = await db.journal_entries.find({"user_id": user["id"]}, {"_id": 0}).sort("date", -1).to_list(1000)
    return docs

@api.post("/journal")
async def create_journal(payload: JournalCreate, user: dict = Depends(get_current_user)):
    entry = JournalEntry(**payload.model_dump(), user_id=user["id"])
    doc = entry.model_dump()
    await db.journal_entries.insert_one(doc)
    doc.pop("_id", None)
    return doc

@api.delete("/journal/{entry_id}")
async def delete_journal(entry_id: str, user: dict = Depends(get_current_user)):
    return await _delete(db.journal_entries, entry_id, user["id"])


# ------------------- AI Journal Generation (Gemini vision) -------------------
from fastapi import UploadFile, File, Form
import base64
import json as _json
import re as _re

GEMINI_API_KEY = os.environ.get("GEMINI_API_KEY", "")
EMERGENT_LLM_KEY = os.environ.get("EMERGENT_LLM_KEY", "")
GEMINI_MODEL = os.environ.get("GEMINI_MODEL", "gemini-3-flash-preview")

AI_JOURNAL_SYSTEM = (
    "You are a warm, evocative writer helping a motorcycle rider journal their ride. "
    "You analyze the attached photos, combine them with the rider's short hints, and produce a rich first-person entry."
)

def _build_ai_prompt(mood: str, duration_min: int, note: str) -> str:
    return (
        "The rider has attached photo(s) from a motorcycle ride. Look at them carefully and infer everything you can — "
        "the road/landscape, likely location, weather, time of day, whether the rider is alone or with others, and approximate distance covered.\n\n"
        f"Rider's hints:\n- Mood: {mood or 'unspecified'}\n- Duration (minutes): {duration_min}\n- Note: {note or '(no note)'}\n\n"
        "Return ONLY a valid JSON object (no markdown fences, no commentary) with EXACTLY these keys:\n"
        "  title (string, short evocative title, 4–8 words),\n"
        "  location (string, place name inferred from image; use a plausible general region like 'Himalayan foothills' if uncertain),\n"
        "  weather (short string like '14° · overcast' or 'clear · warm'),\n"
        "  companions (string: 'Solo' or 'With friends' or 'With group'),\n"
        "  distance_km (integer, best-guess km ridden),\n"
        "  narrative (string, ~120 word rich first-person paragraph about the ride, weaving in what you see).\n"
    )

def _extract_json_block(text: str) -> dict:
    if not text:
        return {}
    m = _re.search(r"\{[\s\S]*\}", text)
    if not m:
        return {}
    try:
        return _json.loads(m.group(0))
    except Exception:
        return {}

async def _call_gemini(images_b64: List[dict], prompt: str, api_key: str) -> str:
    from emergentintegrations.llm.chat import LlmChat, UserMessage, ImageContent
    chat = LlmChat(
        api_key=api_key,
        session_id=f"journal-{uuid.uuid4()}",
        system_message=AI_JOURNAL_SYSTEM,
    ).with_model("gemini", GEMINI_MODEL)
    file_contents = [ImageContent(image_base64=img["b64"]) for img in images_b64]
    msg = UserMessage(text=prompt, file_contents=file_contents)
    return await chat.send_message(msg)

@api.post("/journal/generate")
async def generate_journal(
    mood: str = Form(""),
    duration_min: int = Form(0),
    note: str = Form(""),
    trip_id: str = Form(""),
    photos: List[UploadFile] = File(...),
    user: dict = Depends(get_current_user),
):
    if not photos:
        raise HTTPException(status_code=400, detail="At least one photo required")
    if len(photos) > 3:
        raise HTTPException(status_code=400, detail="Max 3 photos per entry")

    # Encode photos to base64 and build data URLs for storage
    images_b64 = []
    data_urls = []
    for p in photos:
        raw = await p.read()
        if len(raw) > 6 * 1024 * 1024:
            raise HTTPException(status_code=400, detail=f"Photo too large: {p.filename}")
        mime = p.content_type or "image/jpeg"
        if mime not in ("image/jpeg", "image/png", "image/webp"):
            raise HTTPException(status_code=400, detail=f"Unsupported image type: {mime}")
        b64 = base64.b64encode(raw).decode("utf-8")
        images_b64.append({"b64": b64, "mime": mime})
        data_urls.append(f"data:{mime};base64,{b64}")

    prompt = _build_ai_prompt(mood, duration_min, note)

    # Try user's Gemini key first, fall back to Emergent LLM key
    text_response = None
    last_error = None
    for key_name, key in (("GEMINI_API_KEY", GEMINI_API_KEY), ("EMERGENT_LLM_KEY", EMERGENT_LLM_KEY)):
        if not key:
            continue
        try:
            text_response = await _call_gemini(images_b64, prompt, key)
            logger.info("AI journal generated with %s", key_name)
            break
        except Exception as e:
            last_error = f"{key_name}: {e}"
            logger.warning("AI call failed with %s: %s", key_name, e)
            continue

    if not text_response:
        raise HTTPException(status_code=502, detail=f"AI generation failed: {last_error or 'no key available'}")

    parsed = _extract_json_block(text_response)

    entry = JournalEntry(
        user_id=user["id"],
        trip_id=trip_id or None,
        date=datetime.now(timezone.utc).date().isoformat(),
        title=parsed.get("title") or "A ride to remember",
        location=parsed.get("location") or "Unknown road",
        weather=parsed.get("weather") or "",
        companions=parsed.get("companions") or "Solo",
        mood=mood or "",
        distance_km=int(parsed.get("distance_km") or 0),
        duration_min=int(duration_min or 0),
        notes=parsed.get("narrative") or text_response.strip(),
        photos=data_urls,
        ai_generated=True,
    )
    doc = entry.model_dump()
    await db.journal_entries.insert_one(doc)
    doc.pop("_id", None)
    return doc


# ------------------- Packing -------------------
@api.get("/packing")
async def list_packing(user: dict = Depends(get_current_user)):
    return await _list(db.pack_items, user["id"])

@api.post("/packing")
async def create_pack_item(payload: PackItemCreate, user: dict = Depends(get_current_user)):
    item = PackItem(**payload.model_dump(), user_id=user["id"])
    doc = item.model_dump()
    await db.pack_items.insert_one(doc)
    doc.pop("_id", None)
    return doc

@api.put("/packing/{item_id}")
async def toggle_pack_item(item_id: str, payload: dict, user: dict = Depends(get_current_user)):
    existing = await _get(db.pack_items, item_id, user["id"])
    packed = bool(payload.get("packed", not existing.get("packed", False)))
    await db.pack_items.update_one({"id": item_id, "user_id": user["id"]}, {"$set": {"packed": packed}})
    existing["packed"] = packed
    return existing

@api.delete("/packing/{item_id}")
async def delete_pack_item(item_id: str, user: dict = Depends(get_current_user)):
    return await _delete(db.pack_items, item_id, user["id"])


# ------------------- Stats -------------------
@api.get("/stats/summary")
async def stats_summary(user: dict = Depends(get_current_user)):
    trips = await db.trips.find({"user_id": user["id"]}, {"_id": 0}).to_list(1000)
    fuel = await db.fuel_entries.find({"user_id": user["id"]}, {"_id": 0}).to_list(1000)
    expenses = await db.expenses.find({"user_id": user["id"]}, {"_id": 0}).to_list(1000)
    journal = await db.journal_entries.find({"user_id": user["id"]}, {"_id": 0}).to_list(1000)

    total_distance = sum((t.get("distance_km") or 0) for t in trips) + sum((j.get("distance_km") or 0) for j in journal)
    longest = max([t.get("distance_km") or 0 for t in trips] + [j.get("distance_km") or 0 for j in journal] + [0])
    riding_days = len({j.get("date") for j in journal if j.get("date")})
    total_trips = len(trips)

    total_liters = sum((f.get("liters") or 0) for f in fuel)
    mileages = [f.get("computed_mileage") for f in fuel if f.get("computed_mileage")]
    avg_mileage = round(sum(mileages) / len(mileages), 2) if mileages else 0.0

    spend_by_cat: dict = {}
    for e in expenses:
        cat = e.get("category", "Other")
        spend_by_cat[cat] = round(spend_by_cat.get(cat, 0) + (e.get("amount") or 0), 2)

    # distance by month (last 6 months from now)
    now = datetime.now(timezone.utc)
    months = []
    for i in range(5, -1, -1):
        m = (now.month - i - 1) % 12 + 1
        y = now.year + ((now.month - i - 1) // 12)
        months.append((y, m))
    month_map = {f"{y:04d}-{m:02d}": 0 for (y, m) in months}
    for j in journal:
        d = j.get("date", "")
        key = d[:7] if len(d) >= 7 else ""
        if key in month_map:
            month_map[key] += j.get("distance_km") or 0

    return {
        "total_distance": total_distance,
        "longest_ride": longest,
        "riding_days": riding_days,
        "total_trips": total_trips,
        "total_liters": round(total_liters, 2),
        "avg_mileage": avg_mileage,
        "spend_by_category": spend_by_cat,
        "distance_by_month": [{"month": k, "km": v} for k, v in month_map.items()],
    }


@api.get("/")
async def root():
    return {"service": "RideAtlas API", "status": "ok"}


# Register router
app.include_router(api)

app.add_middleware(
    CORSMiddleware,
    allow_credentials=True,
    allow_origins=[FRONTEND_URL, "http://localhost:3000"],
    allow_methods=["*"],
    allow_headers=["*"],
)


@app.on_event("shutdown")
async def shutdown():
    client.close()
