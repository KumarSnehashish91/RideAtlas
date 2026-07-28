import React, { useEffect, useState } from "react";
import client, { formatApiError } from "../lib/api";
import { PageContainer, PageHeader, StatTile, EmptyState } from "../components/Shared";
import { Button } from "../components/ui/button";
import { Input } from "../components/ui/input";
import { Label } from "../components/ui/label";
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger, DialogFooter } from "../components/ui/dialog";
import { PlusIcon, TrashIcon } from "@phosphor-icons/react";
import { toast } from "sonner";

const EMPTY = { date: new Date().toISOString().slice(0, 10), odometer: 0, liters: 0, cost: 0, station: "", fuel_type: "Petrol" };

export default function FuelLog() {
  const [entries, setEntries] = useState([]);
  const [open, setOpen] = useState(false);
  const [form, setForm] = useState(EMPTY);
  const [saving, setSaving] = useState(false);

  const load = () => client.get("/fuel").then((r) => setEntries(r.data)).catch(() => {});
  useEffect(() => { load(); }, []);

  const setField = (k) => (e) => setForm({ ...form, [k]: e?.target ? e.target.value : e });

  const save = async () => {
    setSaving(true);
    try {
      await client.post("/fuel", {
        ...form,
        odometer: Number(form.odometer), liters: Number(form.liters), cost: Number(form.cost),
      });
      toast.success("Fuel entry added");
      setOpen(false); setForm(EMPTY); load();
    } catch (e) { toast.error(formatApiError(e)); }
    finally { setSaving(false); }
  };

  const del = async (id) => {
    await client.delete(`/fuel/${id}`); toast.success("Deleted"); load();
  };

  // Group by month
  const groups = entries.reduce((acc, e) => {
    const key = (e.date || "").slice(0, 7);
    acc[key] = acc[key] || [];
    acc[key].push(e);
    return acc;
  }, {});
  const monthKeys = Object.keys(groups).sort((a, b) => (a < b ? 1 : -1));

  const thisMonth = new Date().toISOString().slice(0, 7);
  const thisMonthEntries = groups[thisMonth] || [];
  const monthCost = thisMonthEntries.reduce((s, e) => s + (e.cost || 0), 0);
  const mileages = entries.filter((e) => e.computed_mileage).map((e) => e.computed_mileage);
  const avgMileage = mileages.length ? (mileages.reduce((a, b) => a + b, 0) / mileages.length).toFixed(1) : "0.0";

  return (
    <PageContainer>
      <PageHeader
        overline="Fuel Log"
        title="Every liter counts."
        description="Log fill-ups and watch mileage trends emerge. We compute km/L for you across bikes."
        right={
          <Dialog open={open} onOpenChange={setOpen}>
            <DialogTrigger asChild>
              <Button className="bg-primary hover:bg-primary/90" data-testid="add-fuel-button"><PlusIcon size={16} className="mr-1.5" /> Add entry</Button>
            </DialogTrigger>
            <DialogContent className="max-w-md">
              <DialogHeader><DialogTitle className="font-display">Add fuel entry</DialogTitle></DialogHeader>
              <div className="space-y-3">
                <div><Label>Date</Label><Input type="date" value={form.date} onChange={setField("date")} data-testid="fuel-form-date" /></div>
                <div><Label>Odometer (km)</Label><Input type="number" value={form.odometer} onChange={setField("odometer")} data-testid="fuel-form-odo" /></div>
                <div><Label>Liters</Label><Input type="number" step="0.01" value={form.liters} onChange={setField("liters")} data-testid="fuel-form-liters" /></div>
                <div><Label>Cost (₹)</Label><Input type="number" step="0.01" value={form.cost} onChange={setField("cost")} data-testid="fuel-form-cost" /></div>
                <div><Label>Station</Label><Input value={form.station} onChange={setField("station")} data-testid="fuel-form-station" placeholder="Shell, Gurugram" /></div>
                <div><Label>Fuel type</Label><Input value={form.fuel_type} onChange={setField("fuel_type")} data-testid="fuel-form-type" /></div>
              </div>
              <DialogFooter>
                <Button variant="outline" onClick={() => setOpen(false)}>Cancel</Button>
                <Button disabled={saving} onClick={save} className="bg-primary hover:bg-primary/90" data-testid="fuel-form-save">
                  {saving ? "Saving…" : "Save entry"}
                </Button>
              </DialogFooter>
            </DialogContent>
          </Dialog>
        }
      />

      <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-8">
        <StatTile label="Avg mileage" value={avgMileage} unit="km/L" tone="primary" testid="fuel-stat-mileage" />
        <StatTile label="This month" value={`₹${monthCost.toLocaleString()}`} testid="fuel-stat-month-cost" />
        <StatTile label="Fill-ups this month" value={thisMonthEntries.length} tone="accent" testid="fuel-stat-fillups" />
      </div>

      {entries.length === 0 ? (
        <EmptyState title="No fuel entries yet" description="Log your first fill-up to start tracking mileage and monthly spend." />
      ) : (
        <div className="space-y-6">
          {monthKeys.map((key) => (
            <div key={key}>
              <div className="overline mb-3">{new Date(key + "-01").toLocaleDateString("en-US", { month: "long", year: "numeric" })}</div>
              <div className="border border-border rounded-md overflow-hidden divide-y divide-border">
                {groups[key].map((e) => (
                  <div key={e.id} className="grid grid-cols-12 gap-3 p-4 hover:bg-muted/30 transition-colors items-center" data-testid={`fuel-row-${e.id}`}>
                    <div className="col-span-4">
                      <div className="font-medium">{e.station || "Fuel stop"}</div>
                      <div className="text-xs text-muted-foreground">{e.date}</div>
                    </div>
                    <div className="col-span-2 text-sm number-tabular">{e.liters} L</div>
                    <div className="col-span-2 text-sm number-tabular text-muted-foreground">{e.odometer?.toLocaleString()} km</div>
                    <div className="col-span-2 text-sm number-tabular text-primary">{e.computed_mileage ? `${e.computed_mileage} km/L` : "—"}</div>
                    <div className="col-span-1 text-right font-semibold number-tabular">₹{e.cost}</div>
                    <div className="col-span-1 text-right">
                      <button onClick={() => del(e.id)} data-testid={`fuel-delete-${e.id}`} className="p-1.5 rounded-md text-muted-foreground hover:text-destructive hover:bg-destructive/10"><TrashIcon size={14} /></button>
                    </div>
                  </div>
                ))}
              </div>
            </div>
          ))}
        </div>
      )}
    </PageContainer>
  );
}
