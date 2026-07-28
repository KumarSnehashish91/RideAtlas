import React, { useEffect, useState } from "react";
import client from "../lib/api";
import { PageContainer, PageHeader, StatTile } from "../components/Shared";
import { BarChart, Bar, XAxis, YAxis, ResponsiveContainer, Tooltip, PieChart, Pie, Cell, Legend } from "recharts";

const COLORS = ["#E25822", "#F59E0B", "#10B981", "#6366F1", "#EC4899"];

export default function Statistics() {
  const [stats, setStats] = useState(null);
  useEffect(() => { client.get("/stats/summary").then((r) => setStats(r.data)).catch(() => {}); }, []);

  if (!stats) return <PageContainer><div className="text-muted-foreground">Loading…</div></PageContainer>;

  const spendData = Object.entries(stats.spend_by_category || {}).map(([name, value]) => ({ name, value }));
  const monthData = (stats.distance_by_month || []).map((m) => ({ month: m.month.slice(5), km: m.km }));

  return (
    <PageContainer>
      <PageHeader
        overline="Your journey"
        title="The atlas so far."
        description="Numbers don't lie. Here's how the road is treating you."
      />

      <div className="grid grid-cols-2 md:grid-cols-4 gap-4 mb-10">
        <StatTile label="Total distance" value={stats.total_distance.toLocaleString()} unit="km" tone="primary" testid="stat-total-distance" />
        <StatTile label="Longest ride" value={stats.longest_ride.toLocaleString()} unit="km" tone="accent" testid="stat-longest-ride" />
        <StatTile label="Riding days" value={stats.riding_days} testid="stat-riding-days" />
        <StatTile label="Total trips" value={stats.total_trips} testid="stat-total-trips-page" />
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-4 mb-6">
        <div className="p-6 border border-border rounded-md bg-card/60">
          <div className="overline mb-4">Distance by month</div>
          <div className="h-64">
            <ResponsiveContainer width="100%" height="100%">
              <BarChart data={monthData}>
                <XAxis dataKey="month" stroke="hsl(var(--muted-foreground))" fontSize={12} />
                <YAxis stroke="hsl(var(--muted-foreground))" fontSize={12} />
                <Tooltip contentStyle={{ background: "hsl(var(--card))", border: "1px solid hsl(var(--border))", borderRadius: 6 }} />
                <Bar dataKey="km" fill="#E25822" radius={[4, 4, 0, 0]} />
              </BarChart>
            </ResponsiveContainer>
          </div>
        </div>

        <div className="p-6 border border-border rounded-md bg-card/60">
          <div className="overline mb-4">Spend breakdown</div>
          {spendData.length ? (
            <div className="h-64">
              <ResponsiveContainer width="100%" height="100%">
                <PieChart>
                  <Pie data={spendData} dataKey="value" nameKey="name" innerRadius={50} outerRadius={90} paddingAngle={2}>
                    {spendData.map((_, i) => <Cell key={i} fill={COLORS[i % COLORS.length]} />)}
                  </Pie>
                  <Tooltip contentStyle={{ background: "hsl(var(--card))", border: "1px solid hsl(var(--border))", borderRadius: 6 }} />
                  <Legend />
                </PieChart>
              </ResponsiveContainer>
            </div>
          ) : (
            <div className="text-sm text-muted-foreground">No expenses yet.</div>
          )}
        </div>
      </div>

      <div className="p-6 border border-border rounded-md bg-card/60">
        <div className="overline mb-4">Achievements</div>
        <div className="grid grid-cols-2 md:grid-cols-4 gap-3">
          {[
            { title: "Iron Rider", ok: stats.longest_ride >= 500 },
            { title: "Mountain Explorer", ok: stats.total_distance >= 1000 },
            { title: "10 Trips", ok: stats.total_trips >= 10 },
            { title: "Century", ok: stats.total_distance >= 100 },
          ].map((a) => (
            <div key={a.title} className={`p-4 border rounded-md ${a.ok ? "border-primary/40 bg-primary/5" : "border-border bg-muted/20"}`}>
              <div className={`font-display text-base font-semibold ${a.ok ? "text-primary" : "text-muted-foreground"}`}>{a.title}</div>
              <div className="text-[10px] mt-1 text-muted-foreground">{a.ok ? "Unlocked" : "Locked"}</div>
            </div>
          ))}
        </div>
      </div>
    </PageContainer>
  );
}
