# RideAtlas — PRD

## Original problem statement
> "i want to build a website named 'RideAtlas' with login using authentication. I have attached docs of mobile application review and build"

User attached: `RideAtlas_PRD.pdf`, `RideAtlas_BRD.pdf`, `RideAtlas_Vision.pdf`, and `RideAtlas-Mockups (1).html` (mobile app spec for a motorcycle touring ecosystem).

## User choices (Feb 2026)
- **Auth**: JWT email/password (custom auth) — no OAuth for v1
- **Modules**: Auth + Dashboard + Garage + Trip Planner + Fuel Log + Service History + Expense Manager + Ride Journal + Statistics + Packing Checklist
- **Landing**: Marketing landing → Sign up → App dashboard
- **Design**: Mockup aesthetic — premium dark-first, terracotta primary, topographic/atlas vibes
- **Maps**: Free/open — using **Leaflet + OpenStreetMap (CartoDB dark tiles)**, no API keys

## Personas (from Vision doc)
- Adventure riders (Marcus Vance) — reliability in zero-cellular zones
- Long-distance tourers (Elena Rostova) — multi-day interstate rides, budget analytics
- Weekend riders — curated loops
- Club leads (David Chen) — shared manifests, packing lists, route exports

## Architecture
- **Backend**: FastAPI + Motor (MongoDB) + PyJWT + bcrypt. Single `server.py` with `/api` prefix. Data model is per-user (user_id filter). **Currently not run** — see "Current run mode" below.
- **Frontend**: React 19 + React Router 7 + Tailwind + Shadcn UI + Phosphor Icons + react-leaflet + Recharts + sonner. AuthContext manages session via httpOnly cookie + Bearer fallback in localStorage.
- **Font**: `Outfit` (headings) + `Manrope` (body). Colors: dark bg `#0F1115`, primary `#E25822` terracotta, accent `#F59E0B` amber.

## Current run mode (frontend-only, no DB/API)
- The FastAPI backend and MongoDB are not wired up for local runs. `frontend/src/lib/api.js` now points `client` at `frontend/src/lib/localBackend.js`, an in-browser mock that mirrors every `server.py` route (auth, bikes, trips, fuel, service, expenses, journal incl. AI-journal generation, packing, stats) and persists to `localStorage`.
- To reconnect the real backend later: point `client` in `api.js` back at an axios instance hitting `REACT_APP_BACKEND_URL`, and run `backend/server.py` against a MongoDB instance with `.env` (`MONGO_URL`, `DB_NAME`, `JWT_SECRET`, `GEMINI_API_KEY`, `FRONTEND_URL`). No page/component code needs to change — they all call through `client.get/post/put/delete`.

## What's implemented (Feb 2026 — v1.0)
- JWT auth (register/login/logout/me) with cookie + bearer
- Marketing landing page (hero, features, lifecycle, personas, CTA, footer)
- Login / Register (split-screen aesthetic with topographic imagery)
- Authenticated app shell (sidebar nav + user chip)
- **Dashboard**: greeting, next-trip card, KPI tiles, primary bike, recent activity, quick actions
- **Garage**: CRUD for bikes with primary bike flag
- **Trip Planner**: multi-day trips with waypoints rendered on Leaflet dark map + polyline route
- **Fuel Log**: fill-ups grouped by month, auto-computes km/L from prior odometer
- **Service History**: log services with cost/center/notes
- **Expenses**: per-trip budget vs spend, category breakdown, split with riders
- **Ride Journal (AI-powered — v1.1)**: user uploads 1–3 photos + picks mood + duration + 1-line note. **Gemini 3 Flash vision** analyzes photos, infers location/weather/companions/distance and writes a ~120-word first-person narrative.
- **Journal Detail (v1.2)**: dedicated `/journal/:id` route with a **magazine-style long-form reader** — hero image over 68vh, sticky top bar (back / brand / delete), display-scale title, drop-cap first paragraph, left rail with location/weather/companions/mood/distance/duration, inline photo gallery with click-to-lightbox, and a signature block.
- **Statistics**: total km, longest ride, riding days, achievements, spend pie, monthly bar chart (Recharts)
- **Packing Checklist**: categorised items with progress bar + toggle

## Backlog
- P1: Document Manager (encrypted vault for insurance/license PDFs)
- P1: Route file parsing (GPX/KML upload → auto-generate waypoints)
- P2: Weather API integration on trip planner (currently manual field)
- P2: PWA/offline shell (mockup guidelines are offline-first)
- P2: Dashboard hero weather widget (currently omitted)
- P2: Shared trips (invite riders to view/edit)
- P2: Achievements engine with more milestones
- P2: Light theme toggle (CSS variables already defined)

## Next tasks
1. Verify all flows end-to-end via testing agent
2. Ship v1 and gather user feedback
