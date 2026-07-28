"""RideAtlas backend API tests - auth, CRUD, data isolation, stats."""
import os
import uuid
import pytest
import requests

BASE_URL = os.environ.get('REACT_APP_BACKEND_URL', 'https://atlas-login-portal.preview.emergentagent.com').rstrip('/')
API = f"{BASE_URL}/api"


def _register(suffix=""):
    email = f"TEST_{uuid.uuid4().hex[:10]}{suffix}@example.com"
    s = requests.Session()
    r = s.post(f"{API}/auth/register", json={"email": email, "password": "test1234", "name": "Test User"})
    assert r.status_code == 200, r.text
    data = r.json()
    s.headers.update({"Authorization": f"Bearer {data['token']}"})
    return s, data, email


@pytest.fixture(scope="module")
def user_a():
    s, data, email = _register("_A")
    return {"session": s, "data": data, "email": email}


@pytest.fixture(scope="module")
def user_b():
    s, data, email = _register("_B")
    return {"session": s, "data": data, "email": email}


# ---- Auth ----
class TestAuth:
    def test_health(self):
        r = requests.get(f"{API}/")
        assert r.status_code == 200
        assert r.json().get("status") == "ok"

    def test_register_login_me(self):
        email = f"TEST_{uuid.uuid4().hex[:8]}@example.com".lower()
        r = requests.post(f"{API}/auth/register", json={"email": email, "password": "test1234", "name": "Reggie"})
        assert r.status_code == 200
        token = r.json()["token"]
        # login
        r2 = requests.post(f"{API}/auth/login", json={"email": email, "password": "test1234"})
        assert r2.status_code == 200
        assert r2.json()["email"] == email
        # me with bearer
        r3 = requests.get(f"{API}/auth/me", headers={"Authorization": f"Bearer {token}"})
        assert r3.status_code == 200
        assert r3.json()["email"] == email

    def test_register_duplicate(self):
        email = f"TEST_{uuid.uuid4().hex[:8]}@example.com"
        requests.post(f"{API}/auth/register", json={"email": email, "password": "test1234", "name": "X"})
        r = requests.post(f"{API}/auth/register", json={"email": email, "password": "test1234", "name": "X"})
        assert r.status_code == 400

    def test_login_bad_password(self):
        email = f"TEST_{uuid.uuid4().hex[:8]}@example.com"
        requests.post(f"{API}/auth/register", json={"email": email, "password": "test1234", "name": "X"})
        r = requests.post(f"{API}/auth/login", json={"email": email, "password": "wrong"})
        assert r.status_code == 401

    def test_unauth_bikes_returns_401(self):
        r = requests.get(f"{API}/bikes")
        assert r.status_code == 401


# ---- Bikes ----
class TestBikes:
    def test_bike_crud(self, user_a):
        s = user_a["session"]
        payload = {"name": "Himalayan", "make": "Royal Enfield", "model": "Himalayan 450", "year": 2024,
                   "cc": 452, "reg_no": "TEST123", "odometer": 1000, "avg_mileage": 30.0, "is_primary": True}
        r = s.post(f"{API}/bikes", json=payload)
        assert r.status_code == 200, r.text
        bike = r.json()
        assert bike["name"] == "Himalayan"
        assert bike["user_id"] == user_a["data"]["id"]
        assert "_id" not in bike
        bike_id = bike["id"]
        # list
        r = s.get(f"{API}/bikes")
        assert r.status_code == 200
        assert any(b["id"] == bike_id for b in r.json())
        # delete
        r = s.delete(f"{API}/bikes/{bike_id}")
        assert r.status_code == 200


# ---- Trips ----
class TestTrips:
    def test_trip_crud(self, user_a):
        s = user_a["session"]
        payload = {"title": "TEST Manali", "start_date": "2026-05-01", "end_date": "2026-05-05",
                   "budget": 20000, "riders": 2, "distance_km": 550,
                   "waypoints": [{"name": "Delhi", "lat": 28.61, "lng": 77.20, "note": ""},
                                 {"name": "Manali", "lat": 32.24, "lng": 77.19, "note": ""}],
                   "days": [{"day": 1, "from_city": "Delhi", "to_city": "Chandigarh", "distance_km": 250}]}
        r = s.post(f"{API}/trips", json=payload)
        assert r.status_code == 200, r.text
        trip = r.json()
        assert trip["title"] == "TEST Manali"
        assert len(trip["waypoints"]) == 2
        tid = trip["id"]
        # get one
        r = s.get(f"{API}/trips/{tid}")
        assert r.status_code == 200
        # delete
        r = s.delete(f"{API}/trips/{tid}")
        assert r.status_code == 200
        r = s.get(f"{API}/trips/{tid}")
        assert r.status_code == 404


# ---- Fuel ----
class TestFuel:
    def test_fuel_computed_mileage(self, user_a):
        s = user_a["session"]
        # first entry
        r1 = s.post(f"{API}/fuel", json={"date": "2026-01-01", "odometer": 5000, "liters": 10.0, "cost": 1000, "station": "IOC"})
        assert r1.status_code == 200
        assert r1.json()["computed_mileage"] in (None,)
        # second entry with higher odometer
        r2 = s.post(f"{API}/fuel", json={"date": "2026-01-10", "odometer": 5300, "liters": 10.0, "cost": 1000, "station": "HP"})
        assert r2.status_code == 200
        assert r2.json()["computed_mileage"] == 30.0
        # cleanup
        for eid in [r1.json()["id"], r2.json()["id"]]:
            s.delete(f"{API}/fuel/{eid}")


# ---- Service ----
class TestService:
    def test_service_crud(self, user_a):
        s = user_a["session"]
        r = s.post(f"{API}/service", json={"date": "2026-01-01", "odometer": 1000, "description": "TEST oil change", "cost": 800})
        assert r.status_code == 200
        rid = r.json()["id"]
        r2 = s.get(f"{API}/service")
        assert any(x["id"] == rid for x in r2.json())
        assert s.delete(f"{API}/service/{rid}").status_code == 200


# ---- Expenses ----
class TestExpenses:
    def test_expense_crud(self, user_a):
        s = user_a["session"]
        r = s.post(f"{API}/expenses", json={"date": "2026-01-01", "category": "Fuel", "amount": 500.0, "note": "TEST"})
        assert r.status_code == 200
        eid = r.json()["id"]
        assert s.delete(f"{API}/expenses/{eid}").status_code == 200


# ---- Journal ----
class TestJournal:
    def test_journal_crud(self, user_a):
        s = user_a["session"]
        r = s.post(f"{API}/journal", json={"date": "2026-01-01", "title": "TEST Ride", "location": "Somewhere",
                                            "distance_km": 100})
        assert r.status_code == 200
        jid = r.json()["id"]
        assert s.delete(f"{API}/journal/{jid}").status_code == 200


# ---- Packing ----
class TestPacking:
    def test_packing_toggle(self, user_a):
        s = user_a["session"]
        r = s.post(f"{API}/packing", json={"category": "Documents", "name": "TEST Passport"})
        assert r.status_code == 200
        pid = r.json()["id"]
        assert r.json()["packed"] is False
        r2 = s.put(f"{API}/packing/{pid}", json={"packed": True})
        assert r2.status_code == 200
        assert r2.json()["packed"] is True
        assert s.delete(f"{API}/packing/{pid}").status_code == 200


# ---- Stats ----
class TestStats:
    def test_stats_summary(self, user_a):
        s = user_a["session"]
        r = s.get(f"{API}/stats/summary")
        assert r.status_code == 200
        d = r.json()
        for k in ["total_distance", "longest_ride", "riding_days", "total_trips",
                  "total_liters", "avg_mileage", "spend_by_category", "distance_by_month"]:
            assert k in d
        assert isinstance(d["distance_by_month"], list)
        assert len(d["distance_by_month"]) == 6


# ---- Data Isolation ----
class TestIsolation:
    def test_user_a_cannot_see_user_b(self, user_a, user_b):
        sa, sb = user_a["session"], user_b["session"]
        # user A creates a bike
        r = sa.post(f"{API}/bikes", json={"name": "TEST-A-Bike", "make": "M", "model": "M", "year": 2024, "cc": 300})
        bike_id = r.json()["id"]
        # user B lists bikes - should not include A's
        rb = sb.get(f"{API}/bikes")
        assert rb.status_code == 200
        assert not any(b["id"] == bike_id for b in rb.json())
        # user B tries to delete - 404
        rdel = sb.delete(f"{API}/bikes/{bike_id}")
        assert rdel.status_code == 404
        sa.delete(f"{API}/bikes/{bike_id}")
