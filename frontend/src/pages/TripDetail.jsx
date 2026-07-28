import React, { useEffect, useMemo, useState } from "react";
import { useParams, useNavigate, Link } from "react-router-dom";
import client, { formatApiError } from "../lib/api";
import { Button } from "../components/ui/button";
import { MapContainer, TileLayer, Marker, Popup, Polyline, useMap } from "react-leaflet";
import L from "leaflet";
import {
  ArrowLeftIcon, TrashIcon, CompassIcon, MapPinIcon, CalendarBlankIcon,
  UsersThreeIcon, CoinsIcon, PathIcon, ThermometerIcon, GasPumpIcon,
  HouseIcon, FlagIcon, NavigationArrowIcon,
} from "@phosphor-icons/react";
import { toast } from "sonner";

// Fix Leaflet default icons
delete L.Icon.Default.prototype._getIconUrl;
L.Icon.Default.mergeOptions({
  iconRetinaUrl: "https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon-2x.png",
  iconUrl: "https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon.png",
  shadowUrl: "https://unpkg.com/leaflet@1.9.4/dist/images/marker-shadow.png",
});

// Numbered pin icon (SVG)
function pinIcon(label, color = "#E25822") {
  const svg = `
    <svg xmlns='http://www.w3.org/2000/svg' width='36' height='48' viewBox='0 0 36 48'>
      <defs><filter id='s' x='-20%' y='-20%' width='140%' height='140%'>
        <feDropShadow dx='0' dy='2' stdDeviation='2' flood-opacity='0.4'/></filter></defs>
      <path filter='url(#s)' d='M18 0C8 0 0 8 0 18c0 13 18 30 18 30s18-17 18-30C36 8 28 0 18 0z' fill='${color}'/>
      <circle cx='18' cy='18' r='11' fill='#0F1115'/>
      <text x='18' y='22' text-anchor='middle' font-family='Outfit, Arial' font-weight='700' font-size='13' fill='#fff'>${label}</text>
    </svg>`;
  return L.divIcon({
    html: svg,
    className: "",
    iconSize: [36, 48],
    iconAnchor: [18, 46],
    popupAnchor: [0, -40],
  });
}

function FitBounds({ points }) {
  const map = useMap();
  useEffect(() => {
    if (points && points.length > 0) {
      const bounds = L.latLngBounds(points.map((p) => [p.lat, p.lng]));
      map.fitBounds(bounds, { padding: [60, 60], maxZoom: 8 });
    }
  }, [points, map]);
  return null;
}

function haversine(a, b) {
  const R = 6371;
  const toRad = (d) => (d * Math.PI) / 180;
  const dLat = toRad(b.lat - a.lat);
  const dLng = toRad(b.lng - a.lng);
  const s = Math.sin(dLat / 2) ** 2 + Math.cos(toRad(a.lat)) * Math.cos(toRad(b.lat)) * Math.sin(dLng / 2) ** 2;
  return Math.round(2 * R * Math.asin(Math.sqrt(s)));
}

export default function TripDetail() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [trip, setTrip] = useState(null);
  const [notFound, setNotFound] = useState(false);

  useEffect(() => {
    let mounted = true;
    setTrip(null); setNotFound(false);
    client.get(`/trips/${id}`)
      .then((r) => { if (mounted) setTrip(r.data); })
      .catch(() => { if (mounted) setNotFound(true); });
    return () => { mounted = false; };
  }, [id]);

  useEffect(() => { window.scrollTo({ top: 0 }); }, [id]);

  const del = async () => {
    if (!window.confirm("Delete this trip? This cannot be undone.")) return;
    try {
      await client.delete(`/trips/${id}`);
      toast.success("Trip deleted");
      navigate("/trips");
    } catch (e) { toast.error(formatApiError(e)); }
  };

  const legs = useMemo(() => {
    if (!trip?.waypoints || trip.waypoints.length < 2) return [];
    const out = [];
    for (let i = 0; i < trip.waypoints.length - 1; i++) {
      const a = trip.waypoints[i], b = trip.waypoints[i + 1];
      out.push({ from: a.name, to: b.name, km: haversine(a, b), idx: i + 1 });
    }
    return out;
  }, [trip]);

  if (notFound) {
    return (
      <div className="fixed inset-0 bg-background flex items-center justify-center text-center p-8 z-10">
        <div>
          <div className="overline mb-3">404 · Off route</div>
          <h1 className="font-display text-3xl font-bold">This trip doesn't exist.</h1>
          <p className="text-muted-foreground mt-2">It may have been deleted or the link is wrong.</p>
          <Button asChild className="mt-6 bg-primary hover:bg-primary/90"><Link to="/trips">Back to trips</Link></Button>
        </div>
      </div>
    );
  }

  if (!trip) return <div className="fixed inset-0 bg-background flex items-center justify-center text-muted-foreground z-10">Loading route…</div>;

  const waypoints = trip.waypoints || [];
  const polyline = waypoints.map((w) => [w.lat, w.lng]);
  const start = waypoints[0];
  const end = waypoints[waypoints.length - 1];
  const totalLegKm = legs.reduce((s, l) => s + l.km, 0);
  const startFmt = trip.start_date ? new Date(trip.start_date).toLocaleDateString("en-US", { month: "short", day: "numeric" }) : "—";
  const endFmt = trip.end_date ? new Date(trip.end_date).toLocaleDateString("en-US", { month: "short", day: "numeric", year: "numeric" }) : "—";

  return (
    <div className="fixed inset-0 z-30 bg-background text-foreground overflow-y-auto">
      {/* Top bar */}
      <header className="sticky top-0 z-50 backdrop-blur-md bg-background/85 border-b border-border/60">
        <div className="max-w-7xl mx-auto px-6 h-14 flex items-center justify-between">
          <button
            onClick={() => navigate("/trips")}
            data-testid="trip-detail-back"
            className="flex items-center gap-2 text-sm text-muted-foreground hover:text-foreground transition-colors"
          >
            <ArrowLeftIcon size={16} /> <span className="hidden sm:inline">All trips</span>
          </button>
          <Link to="/dashboard" className="flex items-center gap-2" data-testid="trip-detail-brand">
            <CompassIcon size={18} weight="duotone" className="text-primary" />
            <span className="font-display text-sm font-bold tracking-tight">RideAtlas · Trip</span>
          </Link>
          <button
            onClick={del}
            data-testid="trip-detail-delete"
            className="flex items-center gap-1.5 text-sm text-muted-foreground hover:text-destructive transition-colors"
          >
            <TrashIcon size={14} /> <span className="hidden sm:inline">Delete</span>
          </button>
        </div>
      </header>

      {/* Hero: title + map */}
      <section className="relative bg-background border-b border-border">
        <div className="max-w-7xl mx-auto px-6 pt-10 pb-6">
          <div className="overline text-primary mb-3">Trip · {startFmt} — {endFmt}</div>
          <h1 className="font-display text-4xl md:text-6xl lg:text-7xl font-extrabold tracking-tighter leading-[1.02]" data-testid="trip-detail-title">
            {trip.title}
          </h1>
          {start && end && (
            <div className="flex items-center gap-2 mt-4 text-lg text-muted-foreground">
              <FlagIcon size={18} weight="duotone" className="text-primary" />
              <span className="font-medium text-foreground">{start.name}</span>
              <NavigationArrowIcon size={16} weight="duotone" className="text-muted-foreground mx-1" />
              <span className="font-medium text-foreground">{end.name}</span>
            </div>
          )}
        </div>

        {/* Map */}
        <div className="max-w-7xl mx-auto px-6 pb-8">
          <div className="relative w-full h-[52vh] md:h-[62vh] max-h-[720px] rounded-md overflow-hidden border border-border" data-testid="trip-detail-map">
            {waypoints.length > 0 ? (
              <MapContainer center={[waypoints[0].lat, waypoints[0].lng]} zoom={6} className="h-full w-full" scrollWheelZoom>
                <TileLayer
                  attribution='&copy; OpenStreetMap contributors &copy; CARTO'
                  url="https://{s}.basemaps.cartocdn.com/dark_all/{z}/{x}/{y}{r}.png"
                />
                <FitBounds points={waypoints} />
                {waypoints.map((w, i) => (
                  <Marker
                    key={i}
                    position={[w.lat, w.lng]}
                    icon={pinIcon(String(i + 1), i === 0 ? "#10B981" : i === waypoints.length - 1 ? "#E25822" : "#F59E0B")}
                  >
                    <Popup>
                      <div className="font-semibold">{i + 1}. {w.name}</div>
                      {w.note && <div className="text-xs mt-1">{w.note}</div>}
                      {i === 0 && <div className="text-xs text-emerald-600 mt-1">Start</div>}
                      {i === waypoints.length - 1 && i !== 0 && <div className="text-xs text-primary mt-1">Destination</div>}
                    </Popup>
                  </Marker>
                ))}
                {polyline.length > 1 && (
                  <Polyline positions={polyline} pathOptions={{ color: "#E25822", weight: 4, opacity: 0.85, dashArray: "8 6" }} />
                )}
              </MapContainer>
            ) : (
              <div className="w-full h-full flex items-center justify-center text-muted-foreground">
                No waypoints on this trip yet.
              </div>
            )}
          </div>
        </div>
      </section>

      {/* Meta strip */}
      <section className="border-b border-border">
        <div className="max-w-7xl mx-auto px-6 py-6 grid grid-cols-2 md:grid-cols-5 gap-6">
          <div>
            <div className="overline text-[10px] flex items-center gap-1.5"><CalendarBlankIcon size={12} weight="duotone" /> Dates</div>
            <div className="font-semibold mt-1.5">{startFmt} — {endFmt}</div>
          </div>
          <div>
            <div className="overline text-[10px] flex items-center gap-1.5"><PathIcon size={12} weight="duotone" /> Distance</div>
            <div className="font-semibold mt-1.5 number-tabular">{trip.distance_km || totalLegKm} km</div>
          </div>
          <div>
            <div className="overline text-[10px] flex items-center gap-1.5"><CoinsIcon size={12} weight="duotone" /> Budget</div>
            <div className="font-semibold mt-1.5 number-tabular">₹{(trip.budget || 0).toLocaleString()}</div>
          </div>
          <div>
            <div className="overline text-[10px] flex items-center gap-1.5"><UsersThreeIcon size={12} weight="duotone" /> Riders</div>
            <div className="font-semibold mt-1.5 number-tabular">{trip.riders || 1}</div>
          </div>
          <div>
            <div className="overline text-[10px] flex items-center gap-1.5"><MapPinIcon size={12} weight="duotone" /> Waypoints</div>
            <div className="font-semibold mt-1.5 number-tabular">{waypoints.length}</div>
          </div>
        </div>
      </section>

      {/* Two-column detail */}
      <section className="max-w-7xl mx-auto px-6 py-10 grid grid-cols-1 lg:grid-cols-12 gap-8">
        {/* Left: Navigation / waypoints */}
        <div className="lg:col-span-5 lg:sticky lg:top-20 lg:self-start">
          <div className="overline mb-4 flex items-center gap-2">
            <NavigationArrowIcon size={12} weight="duotone" className="text-primary" /> Navigation
          </div>
          <ol className="space-y-3" data-testid="trip-detail-waypoint-list">
            {waypoints.map((w, i) => {
              const isStart = i === 0;
              const isEnd = i === waypoints.length - 1 && i !== 0;
              const nextLeg = legs[i];
              return (
                <li key={i} className="relative">
                  <div className="flex items-start gap-3">
                    <div
                      className={`h-9 w-9 rounded-full flex items-center justify-center font-display font-bold shrink-0 text-sm border-2 ${
                        isStart ? "bg-success/10 text-success border-success/40" :
                        isEnd ? "bg-primary/15 text-primary border-primary/50" :
                        "bg-accent/10 text-accent border-accent/40"
                      }`}
                    >
                      {i + 1}
                    </div>
                    <div className="flex-1 pt-1 min-w-0">
                      <div className="flex items-center gap-2 flex-wrap">
                        <div className="font-semibold truncate">{w.name}</div>
                        {isStart && <span className="text-[9px] px-1.5 py-0.5 rounded-full bg-success/15 text-success border border-success/40 uppercase tracking-wider font-semibold">Start</span>}
                        {isEnd && <span className="text-[9px] px-1.5 py-0.5 rounded-full bg-primary/15 text-primary border border-primary/40 uppercase tracking-wider font-semibold">Destination</span>}
                      </div>
                      <div className="text-xs text-muted-foreground mt-0.5 number-tabular">{w.lat.toFixed(4)}, {w.lng.toFixed(4)}</div>
                      {w.note && <div className="text-xs text-muted-foreground mt-1">{w.note}</div>}
                    </div>
                  </div>
                  {nextLeg && (
                    <div className="ml-4 pl-6 py-2 border-l border-dashed border-border text-xs text-muted-foreground flex items-center gap-2">
                      <PathIcon size={12} weight="duotone" />
                      <span className="number-tabular">{nextLeg.km} km</span>
                      <span>·</span>
                      <span>→ {nextLeg.to}</span>
                    </div>
                  )}
                </li>
              );
            })}
          </ol>
        </div>

        {/* Right: Itinerary */}
        <div className="lg:col-span-7">
          <div className="overline mb-4 flex items-center gap-2">
            <CalendarBlankIcon size={12} weight="duotone" className="text-primary" /> Daily itinerary
          </div>
          {(trip.days || []).length === 0 ? (
            <div className="border border-dashed border-border rounded-md p-8 text-center text-muted-foreground text-sm">
              No day-by-day plan on this trip. Add one when creating a new trip using the day|from|to|km format.
            </div>
          ) : (
            <div className="space-y-3">
              {(trip.days || []).map((d) => (
                <div key={d.day} className="p-5 border border-border rounded-md bg-card/60 hover:border-primary/40 transition-colors" data-testid={`trip-detail-day-${d.day}`}>
                  <div className="flex items-center justify-between flex-wrap gap-2">
                    <div className="flex items-center gap-3">
                      <div className="h-9 w-9 rounded-md bg-primary/10 text-primary font-display font-bold flex items-center justify-center text-sm">
                        {d.day}
                      </div>
                      <div>
                        <div className="font-semibold">{d.from_city} <span className="text-muted-foreground mx-1">→</span> {d.to_city}</div>
                        <div className="text-xs text-muted-foreground number-tabular mt-0.5">{d.distance_km} km ride</div>
                      </div>
                    </div>
                    {d.weather_temp != null && (
                      <div className="flex items-center gap-1.5 text-sm text-muted-foreground">
                        <ThermometerIcon size={14} weight="duotone" /> {d.weather_temp}°
                      </div>
                    )}
                  </div>
                  <div className="grid grid-cols-2 gap-4 mt-4 pt-4 border-t border-border text-sm">
                    <div className="flex items-center gap-2 min-w-0">
                      <GasPumpIcon size={14} weight="duotone" className="text-muted-foreground shrink-0" />
                      <div className="min-w-0">
                        <div className="text-[10px] uppercase tracking-wider text-muted-foreground">Fuel stop</div>
                        <div className="truncate">{d.fuel_stop || "—"}</div>
                      </div>
                    </div>
                    <div className="flex items-center gap-2 min-w-0">
                      <HouseIcon size={14} weight="duotone" className="text-muted-foreground shrink-0" />
                      <div className="min-w-0">
                        <div className="text-[10px] uppercase tracking-wider text-muted-foreground">Overnight</div>
                        <div className="truncate">{d.overnight || "—"}</div>
                      </div>
                    </div>
                  </div>
                </div>
              ))}
            </div>
          )}

          {/* Bottom actions */}
          <div className="mt-10 flex flex-col sm:flex-row items-stretch sm:items-center justify-between gap-3">
            <Button variant="outline" asChild className="sm:min-w-[180px]" data-testid="trip-detail-back-bottom">
              <Link to="/trips"><ArrowLeftIcon size={16} className="mr-1.5" /> All trips</Link>
            </Button>
            <Button asChild className="bg-primary hover:bg-primary/90 sm:min-w-[180px]" data-testid="trip-detail-new-trip">
              <Link to="/trips"><PathIcon size={14} weight="duotone" className="mr-1.5" /> Plan another</Link>
            </Button>
          </div>
        </div>
      </section>
    </div>
  );
}
