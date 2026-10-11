import client from "./localBackend";

// RideAtlas is currently running without a live database/API — `client` is an
// in-browser mock (see ./localBackend.js) that mirrors every route in
// backend/server.py and persists to localStorage, so the whole app works the
// same way it will once the real FastAPI + MongoDB backend is reconnected.
//
// To wire the real backend back up: replace `client` below with an axios
// instance pointed at `${process.env.REACT_APP_BACKEND_URL}/api`. No page or
// component needs to change — they all call through client.get/post/put/delete.
export const API = "/api";

export function formatApiError(err) {
  const d = err?.response?.data?.detail;
  if (d == null) return err?.message || "Something went wrong.";
  if (typeof d === "string") return d;
  if (Array.isArray(d)) return d.map((e) => (e?.msg ? e.msg : JSON.stringify(e))).join(" ");
  if (d?.msg) return d.msg;
  return String(d);
}

export default client;
