import React, { useEffect, useMemo, useState } from "react";
import client, { formatApiError } from "../lib/api";
import { PageContainer, PageHeader, EmptyState } from "../components/Shared";
import { Button } from "../components/ui/button";
import { Input } from "../components/ui/input";
import { Label } from "../components/ui/label";
import { Textarea } from "../components/ui/textarea";
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger, DialogFooter } from "../components/ui/dialog";
import { PlusIcon, TrashIcon, MapPinIcon } from "@phosphor-icons/react";
import { toast } from "sonner";
import { MapContainer, TileLayer, Marker, Popup, Polyline } from "react-leaflet";
import L from "leaflet";

// Fix Leaflet default icons (webpack-safe)
delete L.Icon.Default.prototype._getIconUrl;
L.Icon.Default.mergeOptions({
  iconRetinaUrl: "https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon-2x.png",
  iconUrl: "https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon.png",
  shadowUrl: "https://unpkg.com/leaflet@1.9.4/dist/images/marker-shadow.png",
});

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
  const [selected, setSelected] = useState(null);
  const [open, setOpen] = useState(false);
  const [form, setForm] = useState(EMPTY);
  const [saving, setSaving] = useState(false);

  const load = () => client.get("/trips").then((r) => { setTrips(r.data); if (r.data.length && !selected) setSelected(r.data[0]); }).catch(() => {});
  useEffect(() => { load(); /* eslint-disable-next-line */ }, []);

  const setField = (k) => (e) => setForm({ ...form, [k]: e?.target ? e.target.value : e });

  const save = async () => {
    setSaving(true);
    try {
      const waypoints = parseWaypoints(form.waypoints_text);
      const days = parseDays(form.days_text);
      const { data } = await client.post("/trips", {
        title: form.title, start_date: form.start_date, end_date: form.end_date,
        budget: Number(form.budget), riders: Number(form.riders), distance_km: Number(form.distance_km),
        waypoints, days,
      });
      toast.success("Trip created");
      setOpen(false); setForm(EMPTY); setSelected(data); load();
    } catch (e) { toast.error(formatApiError(e)); }
    finally { setSaving(false); }
  };

  const del = async (id) => {
    if (!window.confirm("Delete this trip?")) return;
    await client.delete(`/trips/${id}`); toast.success("Trip deleted");
    if (selected?.id === id) setSelected(null); load();
  };

  const mapCenter = useMemo(() => {
    if (selected?.waypoints?.length) {
      const w = selected.waypoints[0];
      return [w.lat, w.lng];
    }
    return [28.6139, 77.2090];
  }, [selected]);

  const polyline = useMemo(() => (selected?.waypoints || []).map((w) => [w.lat, w.lng]), [selected]);

  return (
    <PageContainer>
      <PageHeader
        overline="Trip Planner"
        title="Chart your next atlas entry."
        description="Build multi-day itineraries with waypoints on an interactive map, budget targets and per-day fuel stops."
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
        <div className="grid grid-cols-1 lg:grid-cols-12 gap-4">
          <div className="lg:col-span-4 space-y-3">
            {trips.map((t) => (
              <button
                key={t.id}
                onClick={() => setSelected(t)}
                data-testid={`trip-card-${t.id}`}
                className={`w-full text-left p-4 border rounded-md transition-colors ${selected?.id === t.id ? "border-primary bg-primary/5" : "border-border bg-card/60 hover:border-primary/40"}`}
              >
                <div className="flex items-center justify-between">
                  <div className="font-display text-lg font-bold">{t.title}</div>
                  <span onClick={(e) => { e.stopPropagation(); del(t.id); }} className="text-muted-foreground hover:text-destructive p-1"><TrashIcon size={14} /></span>
                </div>
                <div className="text-xs text-muted-foreground mt-1">{t.start_date} → {t.end_date}</div>
                <div className="text-xs text-muted-foreground mt-1">₹{t.budget} · {t.distance_km} km · {t.riders} rider{t.riders > 1 ? "s" : ""}</div>
              </button>
            ))}
          </div>

          <div className="lg:col-span-8 border border-border rounded-md bg-card/60 overflow-hidden">
            {selected && (
              <>
                <div className="h-72 md:h-96">
                  <MapContainer center={mapCenter} zoom={6} className="h-full w-full" scrollWheelZoom>
                    <TileLayer
                      attribution='&copy; OpenStreetMap contributors'
                      url="https://{s}.basemaps.cartocdn.com/dark_all/{z}/{x}/{y}{r}.png"
                    />
                    {(selected.waypoints || []).map((w, i) => (
                      <Marker key={i} position={[w.lat, w.lng]}>
                        <Popup><b>{w.name}</b>{w.note ? <div className="text-xs">{w.note}</div> : null}</Popup>
                      </Marker>
                    ))}
                    {polyline.length > 1 && <Polyline positions={polyline} pathOptions={{ color: "#E25822", weight: 3 }} />}
                  </MapContainer>
                </div>
                <div className="p-5">
                  <div className="flex items-center gap-2 mb-4">
                    <MapPinIcon size={18} weight="duotone" className="text-primary" />
                    <div className="font-display text-xl font-bold">{selected.title}</div>
                  </div>
                  <div className="grid grid-cols-3 gap-3 mb-5 text-sm">
                    <div><div className="overline text-[10px]">Budget</div><div className="font-semibold mt-1 number-tabular">₹{selected.budget}</div></div>
                    <div><div className="overline text-[10px]">Distance</div><div className="font-semibold mt-1 number-tabular">{selected.distance_km} km</div></div>
                    <div><div className="overline text-[10px]">Riders</div><div className="font-semibold mt-1 number-tabular">{selected.riders}</div></div>
                  </div>
                  <div className="overline mb-3">Itinerary</div>
                  <div className="space-y-3">
                    {(selected.days || []).map((d) => (
                      <div key={d.day} className="p-3 border border-border rounded-md">
                        <div className="flex items-center justify-between">
                          <div className="font-semibold">Day {d.day} · {d.from_city} → {d.to_city}</div>
                          <div className="text-xs text-muted-foreground">{d.distance_km} km {d.weather_temp != null && `· ${d.weather_temp}°`}</div>
                        </div>
                        <div className="text-xs text-muted-foreground mt-1">Fuel stop · {d.fuel_stop || "—"} · Overnight · {d.overnight || "—"}</div>
                      </div>
                    ))}
                  </div>
                </div>
              </>
            )}
          </div>
        </div>
      )}
    </PageContainer>
  );
}
