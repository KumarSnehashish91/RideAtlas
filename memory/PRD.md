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
- **Backend**: FastAPI + Motor (MongoDB) + PyJWT + bcrypt. Single `server.py` with `/api` prefix. Data model is per-user (user_id filter).
- **Frontend**: React 19 + React Router 7 + Tailwind + Shadcn UI + Phosphor Icons + react-leaflet + Recharts + sonner. AuthContext manages session via httpOnly cookie + Bearer fallback in localStorage.
- **Font**: `Outfit` (headings) + `Manrope` (body). Colors: dark bg `#0F1115`, primary `#E25822` terracotta, accent `#F59E0B` amber.

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
- **Ride Journal**: rich entries (weather/mood/companions/distance/duration/notes)
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
