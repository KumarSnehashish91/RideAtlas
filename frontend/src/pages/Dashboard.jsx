import React, { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import client from "../lib/api";
import { useAuth } from "../context/AuthContext";
import { PageContainer, StatTile, EmptyState } from "../components/Shared";
import { Button } from "../components/ui/button";
import { MapTrifoldIcon, GasPumpIcon, WrenchIcon, MotorcycleIcon, NotebookIcon } from "@phosphor-icons/react";

export default function Dashboard() {
  const { user } = useAuth();
  const [stats, setStats] = useState(null);
  const [bikes, setBikes] = useState([]);
  const [trips, setTrips] = useState([]);
  const [fuel, setFuel] = useState([]);
  const [journal, setJournal] = useState([]);

  useEffect(() => {
    Promise.all([
      client.get("/stats/summary"),
      client.get("/bikes"),
      client.get("/trips"),
      client.get("/fuel"),
      client.get("/journal"),
    ]).then(([s, b, t, f, j]) => {
      setStats(s.data); setBikes(b.data); setTrips(t.data); setFuel(f.data); setJournal(j.data);
    }).catch(() => {});
  }, []);

  const primary = bikes.find((b) => b.is_primary) || bikes[0];
  const nextTrip = trips[0];
  const recent = [
    ...fuel.slice(0, 2).map((f) => ({ kind: "fuel", when: f.date, title: `Fuel logged`, sub: `${f.station || "Station"} · ${f.liters} L` })),
    ...journal.slice(0, 2).map((j) => ({ kind: "journal", when: j.date, title: `Journal entry`, sub: j.title })),
  ].sort((a, b) => (a.when < b.when ? 1 : -1)).slice(0, 4);

  const greetingHour = new Date().getHours();
  const greeting = greetingHour < 12 ? "Good morning" : greetingHour < 18 ? "Good afternoon" : "Good evening";

  return (
    <PageContainer>
      {/* Greeting */}
      <div className="mb-10">
        <div className="overline mb-3">{greeting}</div>
        <h1 className="font-display text-4xl md:text-5xl font-bold tracking-tighter" data-testid="dashboard-greeting">
          {user?.name?.split(" ")[0] || "Rider"}
        </h1>
      </div>

      {/* Top row: Next trip + stats */}
      <div className="grid grid-cols-1 lg:grid-cols-12 gap-4 mb-8">
        <div className="lg:col-span-6 p-6 border border-border rounded-md bg-card/60 relative overflow-hidden">
          <div className="absolute inset-0 bg-topo opacity-40 pointer-events-none" />
          <div className="relative">
            <div className="overline text-primary">Next ride</div>
            {nextTrip ? (
              <>
                <div className="font-display text-3xl font-bold mt-2" data-testid="dashboard-next-trip-title">{nextTrip.title}</div>
                <div className="text-sm text-muted-foreground mt-1">
                  {nextTrip.start_date} → {nextTrip.end_date} · {nextTrip.distance_km || 0} km · ₹{nextTrip.budget || 0}
                </div>
                <Button asChild size="sm" className="mt-4 bg-primary hover:bg-primary/90">
                  <Link to="/trips" data-testid="dashboard-view-trip-btn">View trip</Link>
                </Button>
              </>
            ) : (
              <>
                <div className="font-display text-2xl font-bold mt-2">No trip planned</div>
                <div className="text-sm text-muted-foreground mt-1">Set a destination and pack the panniers.</div>
                <Button asChild size="sm" className="mt-4 bg-primary hover:bg-primary/90">
                  <Link to="/trips" data-testid="dashboard-plan-trip-btn">Plan a ride</Link>
                </Button>
              </>
            )}
          </div>
        </div>
        <div className="lg:col-span-6 grid grid-cols-3 gap-4">
          <StatTile label="Total km" value={(stats?.total_distance ?? 0).toLocaleString()} tone="primary" testid="stat-total-km" />
          <StatTile label="Km/L avg" value={stats?.avg_mileage ?? 0} tone="accent" testid="stat-avg-mileage" />
          <StatTile label="Trips" value={stats?.total_trips ?? 0} testid="stat-total-trips" />
        </div>
      </div>

      {/* Primary bike + recent activity */}
      <div className="grid grid-cols-1 lg:grid-cols-12 gap-4">
        <div className="lg:col-span-7 p-6 border border-border rounded-md bg-card/60">
          <div className="overline mb-4">Primary bike</div>
          {primary ? (
            <div>
              <div className="flex items-start gap-4">
                <div className="p-3 rounded-md bg-primary/10 text-primary">
                  <MotorcycleIcon size={36} weight="duotone" />
                </div>
                <div className="flex-1">
                  <div className="font-display text-2xl font-bold" data-testid="dashboard-primary-bike">{primary.name}</div>
                  <div className="text-sm text-muted-foreground">{primary.make} · {primary.year} · {primary.cc}cc</div>
                </div>
              </div>
              <div className="grid grid-cols-3 gap-4 mt-6 text-sm">
                <div><div className="overline text-[10px]">Odometer</div><div className="font-semibold mt-1 number-tabular">{primary.odometer?.toLocaleString()} km</div></div>
                <div><div className="overline text-[10px]">Avg mileage</div><div className="font-semibold mt-1 number-tabular">{primary.avg_mileage} km/L</div></div>
                <div><div className="overline text-[10px]">Reg</div><div className="font-semibold mt-1">{primary.reg_no || "—"}</div></div>
              </div>
            </div>
          ) : (
            <EmptyState
              title="No bikes yet"
              description="Add your motorcycle to unlock fuel logs, service history and mileage insights."
              action={<Button asChild className="bg-primary hover:bg-primary/90"><Link to="/garage" data-testid="dashboard-add-bike-cta">Add a bike</Link></Button>}
            />
          )}
        </div>
        <div className="lg:col-span-5 p-6 border border-border rounded-md bg-card/60">
          <div className="overline mb-4">Recent activity</div>
          {recent.length ? (
            <ul className="space-y-4">
              {recent.map((r, i) => (
                <li key={i} className="flex items-center gap-3">
                  <div className="p-2 rounded-md bg-muted/60 text-muted-foreground">
                    {r.kind === "fuel" ? <GasPumpIcon size={18} weight="duotone" /> : <NotebookIcon size={18} weight="duotone" />}
                  </div>
                  <div className="flex-1">
                    <div className="text-sm font-medium">{r.title}</div>
                    <div className="text-xs text-muted-foreground">{r.sub}</div>
                  </div>
                  <div className="text-xs text-muted-foreground">{r.when}</div>
                </li>
              ))}
            </ul>
          ) : (
            <div className="text-sm text-muted-foreground">No activity yet. Log a ride or fill-up to start tracking.</div>
          )}
        </div>
      </div>

      {/* Quick links */}
      <div className="mt-8 grid grid-cols-2 md:grid-cols-4 gap-3">
        {[
          { to: "/trips", label: "Plan trip", Icon: MapTrifoldIcon },
          { to: "/fuel", label: "Log fuel", Icon: GasPumpIcon },
          { to: "/service", label: "Log service", Icon: WrenchIcon },
          { to: "/journal", label: "New journal", Icon: NotebookIcon },
        ].map(({ to, label, Icon }) => (
          <Link
            key={to}
            to={to}
            data-testid={`quick-${to.slice(1)}`}
            className="flex items-center gap-3 p-4 border border-border rounded-md bg-card/40 hover:border-primary/50 hover:-translate-y-0.5 transition-transform"
          >
            <Icon size={20} weight="duotone" className="text-primary" />
            <span className="text-sm font-medium">{label}</span>
          </Link>
        ))}
      </div>
    </PageContainer>
  );
}
