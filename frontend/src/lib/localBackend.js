// In-browser mock of the RideAtlas backend (see backend/server.py).
//
// The real app talks to a FastAPI + MongoDB backend over HTTP. For now that
// backend/database isn't running, so this module plays its part entirely in
// the browser: same routes, same data shapes, same per-user isolation —
// persisted to localStorage instead of Mongo. `lib/api.js` exposes it behind
// the same get/post/put/delete interface the real axios client used, so no
// page or component needs to know the difference. To go back to a real
// backend later, swap `client` in `api.js` for an axios instance and delete
// this file — nothing else changes.

const DB_KEY = "ra_local_db";
const TOKEN_KEY = "ra_token";

function uid() {
  if (typeof crypto !== "undefined" && crypto.randomUUID) return crypto.randomUUID();
  return `id-${Date.now().toString(36)}-${Math.random().toString(36).slice(2)}`;
}

function nowIso() {
  return new Date().toISOString();
}

function apiError(status, detail) {
  const err = new Error(detail);
  err.response = { status, data: { detail } };
  return err;
}

function emptyDb() {
  return {
    users: [],
    bikes: [],
    trips: [],
    fuel_entries: [],
    service_records: [],
    expenses: [],
    journal_entries: [],
    pack_items: [],
  };
}

function seedDb() {
  const demo = emptyDb();
  demo.users.push({
    id: uid(),
    email: "demo@rideatlas.com",
    name: "Demo Rider",
    password: "demo1234",
    created_at: nowIso(),
  });
  return demo;
}

function loadDb() {
  try {
    const raw = localStorage.getItem(DB_KEY);
    if (!raw) {
      const seeded = seedDb();
      localStorage.setItem(DB_KEY, JSON.stringify(seeded));
      return seeded;
    }
    return { ...emptyDb(), ...JSON.parse(raw) };
  } catch {
    return seedDb();
  }
}

let db = loadDb();

function saveDb() {
  try {
    localStorage.setItem(DB_KEY, JSON.stringify(db));
  } catch {
    // Storage full or unavailable — keep working from the in-memory copy.
  }
}

// ------------------- Auth helpers -------------------
function makeToken(userId) {
  return btoa(`${userId}:${Date.now()}:${Math.random().toString(36).slice(2)}`);
}

function decodeToken(token) {
  try {
    return atob(token).split(":")[0];
  } catch {
    return null;
  }
}

function currentUser() {
  const token = localStorage.getItem(TOKEN_KEY);
  if (!token) return null;
  const userId = decodeToken(token);
  return db.users.find((u) => u.id === userId) || null;
}

function requireAuth() {
  const user = currentUser();
  if (!user) throw apiError(401, "Not authenticated");
  return user;
}

function publicUser(u) {
  return { id: u.id, email: u.email, name: u.name };
}

// ------------------- Generic per-user collection helpers -------------------
function sortDesc(items, key) {
  return [...items].sort((a, b) => (a[key] < b[key] ? 1 : a[key] > b[key] ? -1 : 0));
}

function listOwned(name, userId, sortKey = "created_at") {
  return sortDesc(db[name].filter((d) => d.user_id === userId), sortKey);
}

function findOwned(name, id, userId) {
  const item = db[name].find((d) => d.id === id && d.user_id === userId);
  if (!item) throw apiError(404, "Not found");
  return item;
}

function deleteOwned(name, id, userId) {
  const idx = db[name].findIndex((d) => d.id === id && d.user_id === userId);
  if (idx === -1) throw apiError(404, "Not found");
  db[name].splice(idx, 1);
  saveDb();
  return { ok: true };
}

// ------------------- Auth routes -------------------
function doRegister(data) {
  const email = (data.email || "").trim().toLowerCase();
  const name = (data.name || "").trim();
  const password = data.password || "";
  if (!email || !name || password.length < 6) {
    throw apiError(422, "Please fill in all fields (password must be at least 6 characters).");
  }
  if (db.users.some((u) => u.email === email)) {
    throw apiError(400, "Email already registered");
  }
  const user = { id: uid(), email, name, password, created_at: nowIso() };
  db.users.push(user);
  saveDb();
  const token = makeToken(user.id);
  return { id: user.id, email: user.email, name: user.name, token };
}

function doLogin(data) {
  const email = (data.email || "").trim().toLowerCase();
  const user = db.users.find((u) => u.email === email);
  if (!user || user.password !== data.password) {
    throw apiError(401, "Invalid email or password");
  }
  const token = makeToken(user.id);
  return { id: user.id, email: user.email, name: user.name, token };
}

function doLogout() {
  return { ok: true };
}

// ------------------- Bikes -------------------
function createBike(data) {
  const user = requireAuth();
  if (data.is_primary) {
    db.bikes.forEach((b) => { if (b.user_id === user.id) b.is_primary = false; });
  }
  const bike = {
    id: uid(),
    user_id: user.id,
    name: data.name || "",
    make: data.make || "",
    model: data.model || "",
    year: Number(data.year) || 0,
    cc: Number(data.cc) || 0,
    reg_no: data.reg_no || "",
    odometer: Number(data.odometer) || 0,
    fuel_type: data.fuel_type || "Petrol",
    avg_mileage: Number(data.avg_mileage) || 0,
    insurance_valid_until: data.insurance_valid_until || null,
    puc_valid_until: data.puc_valid_until || null,
    is_primary: !!data.is_primary,
    image_url: data.image_url || null,
    created_at: nowIso(),
  };
  db.bikes.push(bike);
  saveDb();
  return bike;
}

function updateBike(id, data) {
  const user = requireAuth();
  const existing = findOwned("bikes", id, user.id);
  if (data.is_primary) {
    db.bikes.forEach((b) => { if (b.user_id === user.id) b.is_primary = false; });
  }
  Object.assign(existing, {
    name: data.name ?? existing.name,
    make: data.make ?? existing.make,
    model: data.model ?? existing.model,
    year: data.year != null ? Number(data.year) : existing.year,
    cc: data.cc != null ? Number(data.cc) : existing.cc,
    reg_no: data.reg_no ?? existing.reg_no,
    odometer: data.odometer != null ? Number(data.odometer) : existing.odometer,
    fuel_type: data.fuel_type ?? existing.fuel_type,
    avg_mileage: data.avg_mileage != null ? Number(data.avg_mileage) : existing.avg_mileage,
    insurance_valid_until: data.insurance_valid_until ?? existing.insurance_valid_until,
    puc_valid_until: data.puc_valid_until ?? existing.puc_valid_until,
    is_primary: !!data.is_primary,
    image_url: data.image_url ?? existing.image_url,
  });
  saveDb();
  return existing;
}

// ------------------- Trips -------------------
function createTrip(data) {
  const user = requireAuth();
  const trip = {
    id: uid(),
    user_id: user.id,
    title: data.title || "",
    start_date: data.start_date || "",
    end_date: data.end_date || "",
    budget: Number(data.budget) || 0,
    riders: Number(data.riders) || 1,
    distance_km: Number(data.distance_km) || 0,
    waypoints: data.waypoints || [],
    days: data.days || [],
    created_at: nowIso(),
  };
  db.trips.push(trip);
  saveDb();
  return trip;
}

function updateTrip(id, data) {
  const user = requireAuth();
  const existing = findOwned("trips", id, user.id);
  Object.assign(existing, {
    title: data.title ?? existing.title,
    start_date: data.start_date ?? existing.start_date,
    end_date: data.end_date ?? existing.end_date,
    budget: data.budget != null ? Number(data.budget) : existing.budget,
    riders: data.riders != null ? Number(data.riders) : existing.riders,
    distance_km: data.distance_km != null ? Number(data.distance_km) : existing.distance_km,
    waypoints: data.waypoints ?? existing.waypoints,
    days: data.days ?? existing.days,
  });
  saveDb();
  return existing;
}

// ------------------- Fuel -------------------
function createFuel(data) {
  const user = requireAuth();
  const entry = {
    id: uid(),
    user_id: user.id,
    bike_id: data.bike_id || null,
    date: data.date || "",
    odometer: Number(data.odometer) || 0,
    liters: Number(data.liters) || 0,
    cost: Number(data.cost) || 0,
    station: data.station || "",
    fuel_type: data.fuel_type || "Petrol",
    computed_mileage: null,
    created_at: nowIso(),
  };
  const prior = db.fuel_entries
    .filter((f) => f.user_id === user.id && f.bike_id === entry.bike_id && f.odometer < entry.odometer)
    .sort((a, b) => b.odometer - a.odometer)[0];
  if (prior && entry.liters > 0) {
    const km = entry.odometer - prior.odometer;
    if (km > 0) entry.computed_mileage = Math.round((km / entry.liters) * 100) / 100;
  }
  db.fuel_entries.push(entry);
  saveDb();
  return entry;
}

// ------------------- Service -------------------
function createService(data) {
  const user = requireAuth();
  const rec = {
    id: uid(),
    user_id: user.id,
    bike_id: data.bike_id || null,
    date: data.date || "",
    odometer: Number(data.odometer) || 0,
    description: data.description || "",
    center: data.center || "",
    cost: Number(data.cost) || 0,
    notes: data.notes || "",
    created_at: nowIso(),
  };
  db.service_records.push(rec);
  saveDb();
  return rec;
}

// ------------------- Expenses -------------------
function createExpense(data) {
  const user = requireAuth();
  const exp = {
    id: uid(),
    user_id: user.id,
    trip_id: data.trip_id || null,
    date: data.date || "",
    category: data.category || "Other",
    amount: Number(data.amount) || 0,
    note: data.note || "",
    split_riders: Number(data.split_riders) || 1,
    created_at: nowIso(),
  };
  db.expenses.push(exp);
  saveDb();
  return exp;
}

// ------------------- Journal -------------------
function createJournal(data) {
  const user = requireAuth();
  const entry = {
    id: uid(),
    user_id: user.id,
    trip_id: data.trip_id || null,
    date: data.date || new Date().toISOString().slice(0, 10),
    title: data.title || "",
    location: data.location || "",
    weather: data.weather || "",
    companions: data.companions || "",
    mood: data.mood || "",
    distance_km: Number(data.distance_km) || 0,
    duration_min: Number(data.duration_min) || 0,
    notes: data.notes || "",
    photos: data.photos || [],
    ai_generated: false,
    created_at: nowIso(),
  };
  db.journal_entries.push(entry);
  saveDb();
  return entry;
}

// A stand-in for the real Gemini-vision journal writer (see server.py
// `generate_journal`). It has no model or API key to call from the browser,
// so it composes a plausible entry from the rider's own hints instead of
// actually looking at the photos. Swap this out once the real backend/API
// is wired back up.
const AI_LOCATIONS = [
  "Himalayan foothills", "Western Ghats switchbacks", "Coastal highway",
  "Desert backroads", "Highland plateau", "River valley route",
  "Forest fire road", "Mountain pass",
];
const AI_WEATHERS = [
  "14° · overcast", "22° · clear skies", "18° · light drizzle",
  "27° · warm and dry", "9° · crisp and cold", "20° · golden hour haze",
];
const AI_COMPANIONS = ["Solo", "With friends", "With group"];
const AI_TITLES = [
  "Chasing the Last Light Home", "Switchbacks and Second Thoughts", "The Road That Kept Climbing",
  "Miles of Quiet Company", "Where the Tarmac Runs Out", "A Ridge Line Worth Remembering",
  "Dust, Diesel and Distance", "The Long Way Back",
];

function pick(arr) {
  return arr[Math.floor(Math.random() * arr.length)];
}

function buildAiNarrative({ mood, duration_min, note }) {
  const location = pick(AI_LOCATIONS);
  const weather = pick(AI_WEATHERS);
  const companions = pick(AI_COMPANIONS);
  const distance_km = Math.max(20, Math.round((duration_min || 60) * (0.6 + Math.random() * 0.5)));
  const title = pick(AI_TITLES);
  const weatherMood = weather.split("·")[1]?.trim() || "the weather";
  const moodLine = mood ? `The mood was ${mood.toLowerCase()} the whole way through.` : "";
  const noteLine = note ? ` ${note}.` : "";
  const notes = [
    `The road opened up somewhere past ${location}, and for a while it felt like the only thing that mattered was the next bend.`,
    `${weatherMood[0].toUpperCase()}${weatherMood.slice(1)} held steady, and the engine settled into that rhythm only a long ride can find.`,
    `${moodLine}${noteLine} By the time the light started to fade, the odometer had ticked past another ${distance_km} kilometers, and none of them felt wasted.`,
  ].filter(Boolean).join(" ");
  return { title, location, weather, companions, distance_km, notes };
}

function resizeImageFile(file, maxDim = 1280, quality = 0.78) {
  return new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.onerror = () => reject(new Error("Could not read photo"));
    reader.onload = () => {
      const img = new Image();
      img.onerror = () => reject(new Error("Could not decode photo"));
      img.onload = () => {
        let { width, height } = img;
        if (width > maxDim || height > maxDim) {
          const scale = maxDim / Math.max(width, height);
          width = Math.round(width * scale);
          height = Math.round(height * scale);
        }
        const canvas = document.createElement("canvas");
        canvas.width = width;
        canvas.height = height;
        const ctx = canvas.getContext("2d");
        ctx.drawImage(img, 0, 0, width, height);
        resolve(canvas.toDataURL("image/jpeg", quality));
      };
      img.src = reader.result;
    };
    reader.readAsDataURL(file);
  });
}

async function generateJournal(fd) {
  const user = requireAuth();
  const photoFiles = fd.getAll("photos");
  if (!photoFiles.length) throw apiError(400, "At least one photo required");
  if (photoFiles.length > 3) throw apiError(400, "Max 3 photos per entry");
  for (const f of photoFiles) {
    if (f.size > 6 * 1024 * 1024) throw apiError(400, `Photo too large: ${f.name}`);
  }

  const mood = fd.get("mood") || "";
  const duration_min = Number(fd.get("duration_min") || 0);
  const note = fd.get("note") || "";
  const trip_id = fd.get("trip_id") || "";

  const photos = await Promise.all(photoFiles.map((f) => resizeImageFile(f)));
  const ai = buildAiNarrative({ mood, duration_min, note });

  const entry = {
    id: uid(),
    user_id: user.id,
    trip_id: trip_id || null,
    date: new Date().toISOString().slice(0, 10),
    title: ai.title,
    location: ai.location,
    weather: ai.weather,
    companions: ai.companions,
    mood: mood || "",
    distance_km: ai.distance_km,
    duration_min,
    notes: ai.notes,
    photos,
    ai_generated: true,
    created_at: nowIso(),
  };
  db.journal_entries.push(entry);
  saveDb();
  return entry;
}

// ------------------- Packing -------------------
function createPackItem(data) {
  const user = requireAuth();
  const item = {
    id: uid(),
    user_id: user.id,
    trip_id: data.trip_id || null,
    category: data.category || "Other",
    name: data.name || "",
    packed: !!data.packed,
    created_at: nowIso(),
  };
  db.pack_items.push(item);
  saveDb();
  return item;
}

function togglePackItem(id, data) {
  const user = requireAuth();
  const existing = findOwned("pack_items", id, user.id);
  existing.packed = data && "packed" in data ? !!data.packed : !existing.packed;
  saveDb();
  return existing;
}

// ------------------- Stats -------------------
function statsSummary(userId) {
  const trips = db.trips.filter((t) => t.user_id === userId);
  const fuel = db.fuel_entries.filter((f) => f.user_id === userId);
  const expenses = db.expenses.filter((e) => e.user_id === userId);
  const journal = db.journal_entries.filter((j) => j.user_id === userId);

  const total_distance =
    trips.reduce((s, t) => s + (t.distance_km || 0), 0) +
    journal.reduce((s, j) => s + (j.distance_km || 0), 0);
  const longest_ride = Math.max(
    0,
    ...trips.map((t) => t.distance_km || 0),
    ...journal.map((j) => j.distance_km || 0)
  );
  const riding_days = new Set(journal.filter((j) => j.date).map((j) => j.date)).size;
  const total_trips = trips.length;

  const total_liters = fuel.reduce((s, f) => s + (f.liters || 0), 0);
  const mileages = fuel.map((f) => f.computed_mileage).filter(Boolean);
  const avg_mileage = mileages.length
    ? Math.round((mileages.reduce((a, b) => a + b, 0) / mileages.length) * 100) / 100
    : 0;

  const spend_by_category = {};
  expenses.forEach((e) => {
    const cat = e.category || "Other";
    spend_by_category[cat] = Math.round(((spend_by_category[cat] || 0) + (e.amount || 0)) * 100) / 100;
  });

  const now = new Date();
  const monthMap = {};
  for (let i = 5; i >= 0; i--) {
    const d = new Date(now.getFullYear(), now.getMonth() - i, 1);
    const key = `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, "0")}`;
    monthMap[key] = 0;
  }
  journal.forEach((j) => {
    const key = (j.date || "").slice(0, 7);
    if (key in monthMap) monthMap[key] += j.distance_km || 0;
  });

  return {
    total_distance,
    longest_ride,
    riding_days,
    total_trips,
    total_liters: Math.round(total_liters * 100) / 100,
    avg_mileage,
    spend_by_category,
    distance_by_month: Object.entries(monthMap).map(([month, km]) => ({ month, km })),
  };
}

// ------------------- Route table -------------------
function match(path, pattern) {
  const p = path.split("/").filter(Boolean);
  const t = pattern.split("/").filter(Boolean);
  if (p.length !== t.length) return null;
  const params = {};
  for (let i = 0; i < t.length; i++) {
    if (t[i].startsWith(":")) params[t[i].slice(1)] = decodeURIComponent(p[i]);
    else if (t[i] !== p[i]) return null;
  }
  return params;
}

const routes = [
  ["GET", "/auth/me", () => publicUser(requireAuth())],
  ["POST", "/auth/login", (p, data) => doLogin(data)],
  ["POST", "/auth/register", (p, data) => doRegister(data)],
  ["POST", "/auth/logout", () => doLogout()],

  ["GET", "/bikes", () => listOwned("bikes", requireAuth().id)],
  ["POST", "/bikes", (p, data) => createBike(data)],
  ["PUT", "/bikes/:id", (p, data) => updateBike(p.id, data)],
  ["DELETE", "/bikes/:id", (p) => deleteOwned("bikes", p.id, requireAuth().id)],

  ["GET", "/trips", () => listOwned("trips", requireAuth().id)],
  ["POST", "/trips", (p, data) => createTrip(data)],
  ["GET", "/trips/:id", (p) => findOwned("trips", p.id, requireAuth().id)],
  ["PUT", "/trips/:id", (p, data) => updateTrip(p.id, data)],
  ["DELETE", "/trips/:id", (p) => deleteOwned("trips", p.id, requireAuth().id)],

  ["GET", "/fuel", () => listOwned("fuel_entries", requireAuth().id, "date")],
  ["POST", "/fuel", (p, data) => createFuel(data)],
  ["DELETE", "/fuel/:id", (p) => deleteOwned("fuel_entries", p.id, requireAuth().id)],

  ["GET", "/service", () => listOwned("service_records", requireAuth().id, "date")],
  ["POST", "/service", (p, data) => createService(data)],
  ["DELETE", "/service/:id", (p) => deleteOwned("service_records", p.id, requireAuth().id)],

  ["GET", "/expenses", () => listOwned("expenses", requireAuth().id, "date")],
  ["POST", "/expenses", (p, data) => createExpense(data)],
  ["DELETE", "/expenses/:id", (p) => deleteOwned("expenses", p.id, requireAuth().id)],

  ["POST", "/journal/generate", (p, data) => generateJournal(data)],
  ["GET", "/journal", () => listOwned("journal_entries", requireAuth().id, "date")],
  ["POST", "/journal", (p, data) => createJournal(data)],
  ["GET", "/journal/:id", (p) => findOwned("journal_entries", p.id, requireAuth().id)],
  ["DELETE", "/journal/:id", (p) => deleteOwned("journal_entries", p.id, requireAuth().id)],

  ["GET", "/packing", () => listOwned("pack_items", requireAuth().id)],
  ["POST", "/packing", (p, data) => createPackItem(data)],
  ["PUT", "/packing/:id", (p, data) => togglePackItem(p.id, data)],
  ["DELETE", "/packing/:id", (p) => deleteOwned("pack_items", p.id, requireAuth().id)],

  ["GET", "/stats/summary", () => statsSummary(requireAuth().id)],
];

function dispatch(method, path, data) {
  for (const [m, pattern, handler] of routes) {
    if (m !== method) continue;
    const params = match(path, pattern);
    if (params) return handler(params, data);
  }
  throw apiError(404, `No route for ${method} ${path}`);
}

function delay(ms) {
  return new Promise((resolve) => setTimeout(resolve, ms));
}

async function request(method, path, data) {
  await delay(180);
  try {
    const result = await dispatch(method, path, data);
    return { data: result };
  } catch (err) {
    if (err.response) throw err;
    throw apiError(500, err.message || "Something went wrong.");
  }
}

const client = {
  get: (path) => request("GET", path),
  post: (path, data) => request("POST", path, data),
  put: (path, data) => request("PUT", path, data),
  delete: (path) => request("DELETE", path),
};

export default client;
