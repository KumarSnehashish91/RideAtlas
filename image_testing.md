# Image Testing Playbook (Gemini vision integration on RideAtlas Journal)

## Rules
- Use base64-encoded JPEG/PNG/WEBP for all image tests
- Do NOT use blank, solid-color, or uniform images — model needs visual features
- No GIF/HEIC/SVG. Extract first frame if animated.
- Resize to reasonable bounds before upload (< 4 MB per image)
- MIME must match content — re-detect after transcode

## Endpoint
`POST /api/journal/generate` (auth required)
- multipart/form-data
- Fields: `mood` (str), `duration_min` (int), `note` (str), `photos` (1–3 files)
- Returns: saved `JournalEntry` doc with AI-generated `title`, `location`, `weather`, `companions`, `distance_km`, `notes` (the AI narrative), plus embedded `photos` as data URLs.

## Test approach
1. Register a fresh user → get session
2. Load 1–3 real motorcycle/road photos as bytes (JPEG or PNG). A stock Unsplash motorcycle image works.
3. Send multipart with mood="Great", duration_min=120, note="Twisty ghat road"
4. Expect 200, response has non-empty `title`, `narrative`/`notes` >= 40 chars, `photos` list populated.
5. Verify entry appears in `GET /api/journal`.

## Fallback
If user's `GEMINI_API_KEY` fails, the endpoint automatically retries with `EMERGENT_LLM_KEY` (also loaded in .env).
