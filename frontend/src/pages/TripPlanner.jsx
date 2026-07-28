import React, { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import client, { formatApiError } from "../lib/api";
import { PageContainer, PageHeader, EmptyState } from "../components/Shared";
import { Button } from "../components/ui/button";
import { Input } from "../components/ui/input";
import { Label } from "../components/ui/label";
import { Textarea } from "../components/ui/textarea";
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger, DialogFooter } from "../components/ui/dialog";
import { PlusIcon, TrashIcon, MapTrifoldIcon, CalendarBlankIcon, UsersThreeIcon, CoinsIcon, PathIcon } from "@phosphor-icons/react";
import { toast } from "sonner";

const EMPTY = {
  title: "", start_date: "", end_date: "", budget: 0, riders: 1, distance_km: 0,
  waypoints_text: "Delhi, 28.6139, 77.2090\nManali, 32.2432, 77.1892",
  days_text: "1|Delhi|Chandigarh|260|31|Karnal|Chandigarh\n2|Chandigarh|Manali|300|18|Bilaspur|Manali",
};

function parseWaypoints(text) {
  return text.split("\n").map((l) => l.trim()).filter(Boolean).map((l) => {
    const [name, lat, lng, ...rest] = l.split(",").map((s) => s.trim());
    return { name, lat: parseFloat(lat), lng: parseFloat(lng), note: rest.join(",") };
  }).filter((w) => !Number.isNaN(w.lat) && !Number.isNaN(w.lng));
}

function parseDays(text) {
  return text.split("\n").map((l) => l.trim()).filter(Boolean).map((l) => {
    const [day, from_city, to_city, distance_km, weather_temp, fuel_stop, overnight] = l.split("|").map((s) => (s || "").trim());
    return {
      day: Number(day) || 1,
      from_city: from_city || "",
      to_city: to_city || "",
      distance_km: Number(distance_km) || 0,
      weather_temp: weather_temp ? Number(weather_temp) : null,
      fuel_stop: fuel_stop || "",
      overnight: overnight || "",
    };
  });
}

export default function TripPlanner() {
  const [trips, setTrips] = useState([]);
  const [open, setOpen] = useState(false);
  const [form, setForm] = useState(EMPTY);
  const [saving, setSaving] = useState(false);

  const load = () => client.get("/trips").then((r) => setTrips(r.data)).catch(() => {});
  useEffect(() => { load(); }, []);

  const setField = (k) => (e) => setForm({ ...form, [k]: e?.target ? e.target.value : e });

  const save = async () => {
    if (!form.title.trim()) { toast.error("Title is required"); return; }
    setSaving(true);
    try {
      const waypoints = parseWaypoints(form.waypoints_text);
      const days = parseDays(form.days_text);
      await client.post("/trips", {
        title: form.title, start_date: form.start_date, end_date: form.end_date,
        budget: Number(form.budget), riders: Number(form.riders), distance_km: Number(form.distance_km),
        waypoints, days,
      });
      toast.success("Trip created");
      setOpen(false); setForm(EMPTY); load();
    } catch (e) { toast.error(formatApiError(e)); }
    finally { setSaving(false); }
  };

  const del = async (e, id) => {
    e.preventDefault(); e.stopPropagation();
    if (!window.confirm("Delete this trip?")) return;
    try { await client.delete(`/trips/${id}`); toast.success("Trip deleted"); load(); }
    catch (e2) { toast.error(formatApiError(e2)); }
  };

  return (
    <PageContainer>
      <PageHeader
        overline="Trip Planner"
        title="Chart your next atlas entry."
        description="Build multi-day itineraries with waypoints, budget targets and per-day fuel stops. Click any trip to open its full-screen map."
        right={
          <Dialog open={open} onOpenChange={setOpen}>
            <DialogTrigger asChild>
              <Button className="bg-primary hover:bg-primary/90" data-testid="add-trip-button"><PlusIcon size={16} className="mr-1.5" /> New trip</Button>
            </DialogTrigger>
            <DialogContent className="max-w-2xl">
              <DialogHeader><DialogTitle className="font-display">Plan a new trip</DialogTitle></DialogHeader>
              <div className="grid grid-cols-2 gap-4 max-h-[65vh] overflow-y-auto pr-1">
                <div className="col-span-2"><Label>Title</Label><Input value={form.title} onChange={setField("title")} data-testid="trip-form-title" placeholder="Delhi → Manali" /></div>
                <div><Label>Start date</Label><Input type="date" value={form.start_date} onChange={setField("start_date")} data-testid="trip-form-start" /></div>
                <div><Label>End date</Label><Input type="date" value={form.end_date} onChange={setField("end_date")} data-testid="trip-form-end" /></div>
                <div><Label>Budget (₹)</Label><Input type="number" value={form.budget} onChange={setField("budget")} data-testid="trip-form-budget" /></div>
                <div><Label>Riders</Label><Input type="number" value={form.riders} onChange={setField("riders")} data-testid="trip-form-riders" /></div>
                <div className="col-span-2"><Label>Distance (km)</Label><Input type="number" value={form.distance_km} onChange={setField("distance_km")} data-testid="trip-form-distance" /></div>
                <div className="col-span-2">
                  <Label>Waypoints <span className="text-xs text-muted-foreground">(one per line: name, lat, lng)</span></Label>
                  <Textarea rows={3} value={form.waypoints_text} onChange={setField("waypoints_text")} data-testid="trip-form-waypoints" className="font-mono text-xs" />
                </div>
                <div className="col-span-2">
                  <Label>Days <span className="text-xs text-muted-foreground">(day|from|to|km|weather|fuel_stop|overnight)</span></Label>
                  <Textarea rows={3} value={form.days_text} onChange={setField("days_text")} data-testid="trip-form-days" className="font-mono text-xs" />
                </div>
              </div>
              <DialogFooter>
                <Button variant="outline" onClick={() => setOpen(false)}>Cancel</Button>
                <Button disabled={saving} onClick={save} className="bg-primary hover:bg-primary/90" data-testid="trip-form-save">
                  {saving ? "Saving…" : "Create trip"}
                </Button>
              </DialogFooter>
            </DialogContent>
          </Dialog>
        }
      />

      {trips.length === 0 ? (
        <EmptyState
          title="No trips planned yet"
          description="Plan your first ride with waypoints, daily itinerary and budget."
          action={<Button onClick={() => setOpen(true)} className="bg-primary hover:bg-primary/90" data-testid="empty-add-trip-btn"><PlusIcon size={16} className="mr-1.5" /> New trip</Button>}
        />
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          {trips.map((t) => {
            const firstWp = t.waypoints?.[0]?.name;
            const lastWp = t.waypoints?.[t.waypoints.length - 1]?.name;
            const route = firstWp && lastWp && firstWp !== lastWp ? `${firstWp} → ${lastWp}` : (firstWp || "—");
            return (
              <div key={t.id} className="relative group" data-testid={`trip-card-${t.id}`}>
                <button
                  onClick={(e) => del(e, t.id)}
                  data-testid={`trip-delete-${t.id}`}
                  aria-label="Delete trip"
                  className="absolute top-3 right-3 z-10 p-1.5 rounded-md bg-background/80 backdrop-blur text-muted-foreground hover:text-destructive opacity-0 group-hover:opacity-100 transition-opacity"
                >
                  <TrashIcon size={14} />
                </button>
                <Link
                  to={`/trips/${t.id}`}
                  data-testid={`trip-open-${t.id}`}
                  className="block p-6 border border-border rounded-md bg-card/60 hover:border-primary/50 hover:-translate-y-0.5 transition-all"
                >
                  <div className="flex items-center gap-2 mb-4">
                    <div className="p-2 rounded-md bg-primary/10 text-primary">
                      <MapTrifoldIcon size={18} weight="duotone" />
                    </div>
                    <div className="overline text-[10px]">{t.start_date || "—"} → {t.end_date || "—"}</div>
                  </div>
                  <div className="font-display text-2xl font-bold leading-tight group-hover:text-primary transition-colors line-clamp-2">
                    {t.title}
                  </div>
                  <div className="text-sm text-muted-foreground mt-2">{route}</div>
                  <div className="grid grid-cols-3 gap-3 mt-5 pt-4 border-t border-border text-sm">
                    <div>
                      <div className="flex items-center gap-1 text-muted-foreground text-[10px] uppercase tracking-wide"><PathIcon size={10} /> Km</div>
                      <div className="font-semibold number-tabular mt-0.5">{t.distance_km || 0}</div>
                    </div>
                    <div>
                      <div className="flex items-center gap-1 text-muted-foreground text-[10px] uppercase tracking-wide"><CoinsIcon size={10} /> Budget</div>
                      <div className="font-semibold number-tabular mt-0.5">₹{(t.budget || 0).toLocaleString()}</div>
                    </div>
                    <div>
                      <div className="flex items-center gap-1 text-muted-foreground text-[10px] uppercase tracking-wide"><UsersThreeIcon size={10} /> Riders</div>
                      <div className="font-semibold number-tabular mt-0.5">{t.riders || 1}</div>
                    </div>
                  </div>
                  <div className="flex items-center gap-1 mt-4 text-xs text-primary font-medium opacity-0 group-hover:opacity-100 transition-opacity">
                    <CalendarBlankIcon size={12} weight="duotone" /> Open trip →
                  </div>
                </Link>
              </div>
            );
          })}
        </div>
      )}
    </PageContainer>
  );
}
