# RideAtlas — Web Application Scope & PRD

*Derived from the RideAtlas Android app (Kotlin / Jetpack Compose, package `com.rideatlas.app`), snapshot dated Aug 2026.*

## 1. Purpose

RideAtlas today lives as a native Android companion for motorcyclists — garage management, fuel/service/expense tracking, trip planning, a ride journal, and a live GPS ride-tracking mode (Start Ride → live map/gauges → Complete Ride → summary).

This document scopes a **web application** that gives riders the same ownership, planning, and record-keeping experience from a browser (desktop or laptop-first, responsive to tablet). The web app mirrors mobile feature parity for every module **except** live, sensor-driven ride tracking:

- **"Start Ride" and "Complete Ride" are excluded.** These depend on continuous GPS, a foreground Android service, and a phone mounted on the handlebar — none of which make sense in a browser tab.
- **In their place**, the web app gets a **"Log a Ride" manual-entry flow** — the rider fills in distance, duration, average/max speed (optional), route description, and date/time after the fact, or edits/completes a ride that was started on the phone and later synced. This keeps every downstream feature that depends on a completed ride (Journal entries, ride-linked Fuel/Expense records, Trip completion, Statistics, Community posts) fully functional on web.
- Everything else a rider can do standing still — before, after, or between rides — is in scope for the web app: planning, logging, reviewing, organizing, and analyzing.

## 2. Guiding Principles

1. **Feature parity where it makes sense.** Every mobile module gets a web equivalent unless it's fundamentally tied to phone hardware (GPS tracking, camera capture, offline map tile downloads for turn-by-turn nav).
2. **Web-appropriate adaptations, not a lesser clone.** Where mobile optimizes for one-thumb, on-the-go input, web should optimize for what it's good at: bulk data entry, spreadsheet-style tables, multi-column forms, drag-and-drop file upload, printing/exporting (PDF statements, CSV exports), and larger data visualizations.
3. **Same data model.** The web app's entities are the same shape as the mobile domain models (see §5) so a future sync/API layer (§7) can serve both clients from one backend without translation layers.
4. **Desktop/laptop-first, not desktop-only.** Primary target is a rider planning a trip or reconciling expenses at a laptop; layouts should still degrade gracefully to a tablet.
5. **Photos and documents are uploaded, not captured.** No live camera capture UI is required on web — file picker / drag-and-drop upload replaces it everywhere the mobile app calls the camera.

## 3. Module Scope Summary

| # | Module | Mobile Capability | Web Scope | Notes |
|---|--------|-------------------|-----------|-------|
| 1 | **Auth** | Local on-device account (register/login/guest) | Full — register, login, logout, password reset, session/JWT | Becomes the real multi-device identity once the API exists |
| 2 | **Dashboard** | Greeting, quick actions, next trip, weather, recent activity | Full, with quick actions minus Start Ride | "Start Ride" quick action replaced by "Log a Ride" |
| 3 | **Garage** (bikes) | Add/edit bikes, specs, photos, accessories, active bike | Full | Multi-photo gallery upload, spec sheet, accessory list, service/insurance/PUC/RC expiry tracking |
| 4 | **Fuel Log** | Fill-up entries, full/partial tank, mileage calc, analytics | Full | Includes receipt photo upload, station/location, mileage & cost-per-km analytics, charts |
| 5 | **Service History** | Service records, cost breakdown, invoice photo | Full | Includes labour/parts split, warranty claims, invoice upload |
| 6 | **Expense Manager** | Ad-hoc costs by category, trip-linked or standalone | Full | Category breakdown, receipt upload, trip attribution |
| 7 | **Ride Journal** | Entries linked to a completed ride/trip, mood, photos, AI narrative | Full, ride-link optional | Entry can attach to a manually logged ride, a trip, or stand alone; AI narrative generation retained (server-side LLM call) |
| 8 | **Trip Planner** | Multi-day itinerary, hotels, fuel/rest stops, attractions, packing, AI "Plan My Ride" | Full | Itinerary day-by-day builder, hotel/stop/attraction sub-lists, AI-assisted planning via Ollama/Qwen backend |
| 9 | **Packing Checklist** | Per-trip checklist, templates (Camping/Weekend/Ladakh/Daily) | Full | Same templates, drag-to-reorder, category grouping |
| 10 | **Documents Vault** | Insurance/PUC/RC/License/etc. with expiry reminders | Full | PDF/image upload, expiry-driven reminder badges |
| 11 | **Maintenance Reminders** | Rule-based (interval km/days) + auto-derived (service, insurance, PUC, RC) | Full | Same rule engine; "mark done" resets baseline |
| 12 | **Ride Events** | Planned group rides, invite circle, RSVP | Full | Organizer view; RSVP tracking against Riding Buddies |
| 13 | **Community / Circle** | Rider profile, posts, riding buddies, comments, likes | Full | Feed, profile, circle management; visibility (Public/Circle/Private) |
| 14 | **Explorer** (curated rides) | Browse/search/filter curated India rides, AI-explained recommendations | Full (read/browse) | Powers "Plan My Ride"; ride detail pages with fuel/food/photo/accommodation/permit/packing data |
| 15 | **Offline Maps** | Download MapLibre tile regions for offline nav | **Excluded** | Not meaningful in a browser; web shows trip routes on an always-online map instead |
| 16 | **Weather** | Live forecast card via Open-Meteo, ride suitability badge | Full | Same on Dashboard and Trip Planner |
| 17 | **Statistics** | Distance, spend, achievements, trends, bike comparison | Full | Larger dashboards possible on web (multi-chart layouts) |
| 18 | **Settings** | Units, currency, theme, language, quick-action visibility | Full | Theme (light/dark), distance/fuel units, currency, language |
| 19 | **Active Ride (live tracking)** | GPS tracking, live gauges, route recording, Start/Complete Ride | **Excluded** — replaced by "Log a Ride" manual entry | See §4 |

## 4. Ride Handling on Web — "Log a Ride"

Mobile's `Ride` entity (status ACTIVE/COMPLETED, live GPS distance/speed/route polyline) is created only by pressing Start Ride. On web, a `Ride` record can only ever be created **already completed**, via a manual entry form:

**Log a Ride form fields:**
- Bike (required, from Garage)
- Linked Trip (optional)
- Date & start time / end time (or duration directly)
- Distance (km) — required
- Average speed / max speed (optional, if the rider knows them)
- Route description or start–end locations (free text; no polyline capture)
- Weather condition (optional, can pull from Weather module for that date/location)
- Notes

Once saved, this Ride behaves exactly like a mobile-completed ride for every downstream feature:
- A Journal entry can be written against it.
- Fuel/Expense entries can be tagged to it.
- If linked to a Trip, it sets `Trip.hasRideCompleted = true` the same way mobile's `completeRide()` does, driving the same `TripStatus` derivation (Draft → Planned → Active → Completed/Cancelled) described in `Trip.status()`.
- It appears in Statistics and can be shared as a Community post.

**Explicitly out of scope for web:** live map rendering during a ride, live speed gauge, GPS route recording/polyline capture, background location service, turn-by-turn navigation, offline map region downloads.

## 5. Data Model (shared with mobile)

All entities below map 1:1 to the mobile domain models so the same backend can eventually serve both clients.

### 5.1 Identity
- **RiderAccount** — id, fullName, username, email, mobile, passwordHash/salt, createdAt
- **RiderProfile** (Community) — displayName, bio, homeCity, avatarUri, ridingSinceYear, isPublic, showLocation/showStats/showBikes

### 5.2 Garage
- **Motorcycle** — brand, model, variant, year, engineCc, registrationNumber, vin, engineNumber, colour, fuelType, transmission, nickname, currentOdometerKm, insurance/PUC/RC/warranty expiry dates, purchaseDate/price/location, isActive, isFavorite, isArchived, powerBhp, torqueNm, tankCapacityLitres, ratedMileageKmPerLitre, kerbWeightKg, seatHeightMm, tyre sizes, batterySpec, chainType, oilCapacityLitres, tyre pressures
- **BikePhoto** — bikeId, uri, sortOrder, isPrimary
- **Accessory** — bikeId, category (Helmet/Crash Guard/Top Box/Panniers/Fog Lamps/Phone Mount/GPS/Tyres/Custom), name, cost, purchaseDate, notes

### 5.3 Fuel & Cost Tracking
- **FuelLogEntry** — bikeId, rideId?, date/time, odometerKm, tripMeterKm, litres, pricePerLitre, cost, isFullTank, fuelType, station, stationLocation, paymentMethod, receiptPhotoUri, notes, lat/lng, distanceKm (derived), mileageKmPerLitre (derived), costPerKm (derived)
- **ServiceEntry** — bikeId, date, odometerKm, cost, garage, serviceTypes (multi-select from 22 types), category (Periodic/Emergency/DIY/Dealer/Custom), workshopAddress, mechanic, description, labourCost, partsCost, invoiceNumber, invoicePhotoUri, warrantyClaim, expectedNextServiceDate/OdometerKm, lat/lng
- **Expense** — bikeId, tripId?, rideId?, category (Fuel/Service/Repairs/Insurance/PUC/Registration/Accessories/Parking/Toll/Hotel/Food/Miscellaneous), amount, date, vendor, location, paymentMethod, notes, receiptPhotoUri, odometerKm, lat/lng

### 5.4 Trips
- **Trip** — name, startDate, endDate, startLocation, destination, budget, distanceKm, estimatedDurationMinutes, notes, riderCount, coverPhotoUri, bikeId, isCancelled, isDraft, hasStarted, hasRideCompleted, aiBriefingJson, explorerRideSlug
- **ItineraryDay** — tripId, dayNumber, fromLocation, toLocation, distanceKm, date, waypoints[], durationMinutes, notes
- **Hotel** — tripId, name, location, checkInDate, checkOutDate, cost, notes
- **FuelStop / RestStop** — tripId, stopNumber, location, notes
- **Attraction** — tripId, name, location, notes, visited
- **PackingItem** — tripId, category, name, isPacked, sortOrder (seeded from PackingTemplate: Camping / Weekend Ride / Ladakh Ride / Daily Ride)

### 5.5 Rides & Journal
- **Ride** (web-restricted to manual/completed entry — see §4) — tripId?, bikeId, status, startedAt, endedAt, durationMillis, distanceKm, avgSpeedKmh, maxSpeedKmh, elevationGainM, weatherCondition/tempC, routePolyline (nullable on web), movingMillis
- **JournalEntry** — bikeId, tripId?, rideId?, title, date, notes, mood, distanceKm, photoUris[], riderInputRaw, aiGeneratedStory

### 5.6 Documents & Reminders
- **Document** — bikeId?, type (Bike Insurance/PUC/Registration/Driving License/Pollution Certificate/Permit/Other), title, documentNumber, issueDate, expiryDate, reminderEnabled, photoUri, pdfUri, notes, cost
- **ReminderRule** — bikeId, category (Oil/Chain/Brake Pads/Tyres/Air Filter/Coolant/Battery/Custom), customLabel, intervalKm, intervalDays, baselineOdometerKm, baselineDate, notes
- **MaintenanceReminder** (computed, not stored) — bikeId, category, title, detail, severity (Overdue/Due Soon) — derived from ReminderRule + Service/Document history

### 5.7 Community & Events
- **RidingBuddy** — name, homeCity, avatarUri, notes, isFollowing, remoteRiderId, addedAt
- **CommunityPost** — tripId?, journalEntryId?, caption, photoUris[], location, distanceKm, visibility (Public/Circle/Private), createdAt
- **CommunityComment** — postId, buddyId?, authorName, text, createdAt
- **RideEvent** — title, description, startAt, endAt, meetingPoint, destination, tripId?, bikeId?, coverPhotoUri, isCancelled
- **EventInvitee** — eventId, buddyId, rsvpStatus

### 5.8 Explorer (curated ride library)
- **ExplorerRide** — title, country/state/district/region, origin, destination, intermediateStops[], lat/lng, distanceKm, estimatedDurationHours, recommendedTripDays, roadType, difficulty, bestSeason/avoidSeason, scenic/adventure/photography/food ratings, roadCondition, fuel/medical/network availability, campingAllowed, permitRequired, offRoadPercentage, elevation stats, estimatedFuelCost/Budget, minimumExperience, packingTemplate, weatherSummary, hero/gallery images, aiSummary, categories/motorcycleRecommendations/tags
- **ExplorerRideDetails** — fuelStops, foodStops, photoSpots, accommodations, emergencyPoints, permits, packingItems (each a sub-list scoped to one ride)

### 5.9 Weather & Settings
- **WeatherForecast** — locationLabel, temperatureC, feelsLikeC, condition, highC/lowC, windKmh, humidityPercent, rainProbabilityPercent, sunrise/sunset, hourly[], suitability (Excellent/Good/Rain Expected/High Wind/Extreme)
- **AppSettings** — riderName, themeMode, distanceUnit (km/mi), fuelUnit (L/gal), currency (INR/USD/EUR/GBP), language (English/Hindi), maintenanceRemindersEnabled, preferredMapStyle, visibleQuickActionNames

### 5.10 AI-assisted planning (Plan My Ride)
- **RidePlanningInputs** — companionType, interests[], timeAvailable, ridingStyle
- **TripRecommendation** — destination, region, summary, difficulty, roadType, bestSeason, scenicRating, fuelEstimate, budgetEstimate, weatherOverview, recommendationReason, rideSlug?
- **TripBriefing** — destination, origin, distanceKm, rideDurationMinutes, motorcycle, weather, fuelEstimate, budgetEstimate, roadCondition, difficulty, scenicStops/food/hotels/photographySpots/packingSuggestions/safetyAdvice, aiNarrative
- **RideStory** (post-ride AI recap, generated from a manually logged ride) — journeySummary, highlights, interestingStatistics, memorableMoments, ridingInsights, safetyObservations, futureSuggestions, narrativeText
- **JournalDraft** — title, mood, narrativeText (AI-drafted from a short rider prompt)

## 6. Web-Specific UX Additions (beyond mobile parity)

- **Bulk/table views**: Fuel Log, Service History, and Expenses get a spreadsheet-style table view with inline edit, sort, and multi-select delete — mobile's card-list UX doesn't translate well to a wide screen.
- **CSV / PDF export**: Fuel Log, Service History, Expenses, and Statistics should be exportable (CSV for records, PDF for a shareable trip/expense statement).
- **Drag-and-drop file upload** for receipts, invoices, documents, and bike/journal photos (replaces the mobile camera capture flow).
- **Multi-column forms** for data-heavy entities (Motorcycle spec sheet, Trip itinerary builder) instead of mobile's single-column wizard steps.
- **Print-friendly Packing Checklist** view.

## 7. Backend Scope — ASP.NET Core Web API

The web frontend (and, longer-term, the mobile app once cloud sync ships) will be served by a single ASP.NET Core Web API following **Clean/Onion Architecture**, matching your existing backend conventions.

### 7.1 Solution layout

```
RideAtlas.sln
├── RideAtlas.Domain            → Entities, enums, value objects (mirrors §5), domain rules
│                                  (e.g. Trip.Status() derivation, mileage/reminder calculators)
├── RideAtlas.Application        → Use cases (CQRS via MediatR), DTOs, validators (FluentValidation),
│                                  interfaces for repositories & external services (IFileStorage,
│                                  IWeatherService, IAiPlanningService)
├── RideAtlas.Infrastructure     → EF Core + SQL Server repositories, external service implementations
│                                  (Open-Meteo weather client, Ollama/Qwen AI client, file storage),
│                                  background jobs (Hangfire/Quartz for reminder recompute)
├── RideAtlas.Api                → Controllers, JWT auth (ASP.NET Identity), API versioning,
│                                  Swagger/OpenAPI, request/response DTO mapping (Mapster/AutoMapper)
└── RideAtlas.Tests              → Unit tests (domain/application), integration tests (API + Testcontainers)
```

### 7.2 Cross-cutting concerns

- **Auth**: ASP.NET Core Identity + JWT bearer tokens; refresh tokens for persistent web sessions. One `RiderAccount` → many devices in the future.
- **Persistence**: EF Core Code-First against SQL Server, one migration history shared by web and (later) mobile sync.
- **File storage**: Abstracted `IFileStorage` interface — local disk for dev, Azure Blob Storage (or S3-compatible) for production. Applies to bike photos, receipts, invoices, document PDFs, journal photos, community post photos.
- **Background jobs**: Recurring job to recompute `MaintenanceReminder`s (derived, not stored) and flag Document expiries; nightly digest email optional.
- **AI integration**: `IAiPlanningService` abstraction wrapping the self-hosted Ollama/Qwen model for "Plan My Ride" briefings, AI journal narrative generation, and post-ride story generation — same contract the mobile app's `AiOrchestrator` already expects (`RidePlanningInputs → TripRecommendation[]`, `→ TripBriefing`, ride/journal → `RideStory`/`JournalDraft`).
- **Weather**: `IWeatherService` wrapping Open-Meteo (no API key), same shape as mobile's `WeatherRepository`.
- **Validation**: FluentValidation per command/query in the Application layer.
- **API style**: REST, resource-oriented, `/api/v1/...`, paginated list endpoints, soft-delete where mobile uses `isArchived`/`isCancelled` flags.

### 7.3 Controller / endpoint groups (one per module)

| Controller | Key endpoints |
|---|---|
| `AuthController` | register, login, refresh, logout, me |
| `BikesController` | CRUD, set-active, archive, photos, accessories |
| `FuelLogController` | CRUD, list (paged/filtered by bike/date), analytics (mileage trend, cost/km) |
| `ServiceHistoryController` | CRUD, list, analytics (spend by type, upcoming from reminders) |
| `ExpensesController` | CRUD, list, category breakdown, trip-scoped list |
| `TripsController` | CRUD, itinerary days, hotels, fuel/rest stops, attractions, status transitions |
| `RidesController` | create (manual log), edit, list, link-to-trip, complete-trip-if-linked |
| `JournalController` | CRUD, AI-generate-narrative, photo upload |
| `PackingController` | CRUD per trip, apply-template |
| `DocumentsController` | CRUD, upload, expiring-soon query |
| `RemindersController` | rule CRUD, computed-reminders query, mark-done |
| `EventsController` | CRUD, invitees, RSVP |
| `CommunityController` | posts CRUD, comments, likes, buddies CRUD, profile |
| `ExplorerController` | search/filter rides, ride detail, ai-explain |
| `WeatherController` | forecast-by-location |
| `StatisticsController` | summary, trends, bike comparison, achievements |
| `SettingsController` | get/update AppSettings |
| `AiPlanningController` | plan-my-ride (phase 1 + phase 2 briefing), ride-story, journal-draft |

### 7.4 Suggested build order

1. Domain + Application skeleton, Auth, Bikes (Garage) — everything else foreign-keys to a bike.
2. Fuel Log, Service History, Expenses (the three high-frequency logging modules).
3. Trips + Packing (planning modules) + Rides (manual log) — enables Journal.
4. Journal, Documents, Reminders.
5. Statistics (reads across everything above).
6. Explorer + AI Planning integration (Ollama/Qwen).
7. Community + Events.
8. Weather, Settings, polish, exports.

## 8. Explicitly Out of Scope (Web v1)

- Live GPS ride tracking (Start Ride / Complete Ride), live speed gauge, route recording.
- Offline map tile downloads and offline turn-by-turn navigation.
- Native camera capture (replaced by file upload everywhere).
- Push notifications (web notifications/email digest can be a later addition).
