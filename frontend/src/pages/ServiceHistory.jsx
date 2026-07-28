import React, { useEffect, useState } from "react";
import client, { formatApiError } from "../lib/api";
import { PageContainer, PageHeader, StatTile, EmptyState } from "../components/Shared";
import { Button } from "../components/ui/button";
import { Input } from "../components/ui/input";
import { Label } from "../components/ui/label";
import { Textarea } from "../components/ui/textarea";
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger, DialogFooter } from "../components/ui/dialog";
import { PlusIcon, TrashIcon, WrenchIcon } from "@phosphor-icons/react";
import { toast } from "sonner";

const EMPTY = { date: new Date().toISOString().slice(0, 10), odometer: 0, description: "", center: "", cost: 0, notes: "" };

export default function ServiceHistory() {
  const [records, setRecords] = useState([]);
  const [open, setOpen] = useState(false);
  const [form, setForm] = useState(EMPTY);
  const [saving, setSaving] = useState(false);

  const load = () => client.get("/service").then((r) => setRecords(r.data)).catch(() => {});
  useEffect(() => { load(); }, []);

  const setField = (k) => (e) => setForm({ ...form, [k]: e?.target ? e.target.value : e });

  const save = async () => {
    setSaving(true);
    try {
      await client.post("/service", { ...form, odometer: Number(form.odometer), cost: Number(form.cost) });
      toast.success("Service logged");
      setOpen(false); setForm(EMPTY); load();
    } catch (e) { toast.error(formatApiError(e)); }
    finally { setSaving(false); }
  };

  const del = async (id) => { await client.delete(`/service/${id}`); toast.success("Deleted"); load(); };

  const totalSpent = records.reduce((s, r) => s + (r.cost || 0), 0);
  const lastOdo = records[0]?.odometer || 0;

  return (
    <PageContainer>
      <PageHeader
        overline="Service History"
        title="Never miss a service beat."
        description="Every oil change, chain lube and tyre swap — timestamped and searchable."
        right={
          <Dialog open={open} onOpenChange={setOpen}>
            <DialogTrigger asChild>
              <Button className="bg-primary hover:bg-primary/90" data-testid="add-service-button"><PlusIcon size={16} className="mr-1.5" /> Log service</Button>
            </DialogTrigger>
            <DialogContent className="max-w-md">
              <DialogHeader><DialogTitle className="font-display">Log a service event</DialogTitle></DialogHeader>
              <div className="space-y-3">
                <div><Label>Date</Label><Input type="date" value={form.date} onChange={setField("date")} data-testid="service-form-date" /></div>
                <div><Label>Odometer (km)</Label><Input type="number" value={form.odometer} onChange={setField("odometer")} data-testid="service-form-odo" /></div>
                <div><Label>Description</Label><Input value={form.description} onChange={setField("description")} data-testid="service-form-desc" placeholder="Engine oil + filter" /></div>
                <div><Label>Service center</Label><Input value={form.center} onChange={setField("center")} data-testid="service-form-center" placeholder="RE Service Center" /></div>
                <div><Label>Cost (₹)</Label><Input type="number" value={form.cost} onChange={setField("cost")} data-testid="service-form-cost" /></div>
                <div><Label>Notes</Label><Textarea rows={2} value={form.notes} onChange={setField("notes")} data-testid="service-form-notes" /></div>
              </div>
              <DialogFooter>
                <Button variant="outline" onClick={() => setOpen(false)}>Cancel</Button>
                <Button disabled={saving} onClick={save} className="bg-primary hover:bg-primary/90" data-testid="service-form-save">
                  {saving ? "Saving…" : "Save"}
                </Button>
              </DialogFooter>
            </DialogContent>
          </Dialog>
        }
      />

      <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-8">
        <StatTile label="Total spent" value={`₹${totalSpent.toLocaleString()}`} tone="primary" testid="service-stat-total" />
        <StatTile label="Service events" value={records.length} testid="service-stat-count" />
        <StatTile label="Last recorded odo" value={`${lastOdo.toLocaleString()} km`} tone="accent" testid="service-stat-last-odo" />
      </div>

      {records.length === 0 ? (
        <EmptyState title="No service records yet" description="Every oil change, filter swap, tyre replacement — log them here." />
      ) : (
        <div className="space-y-3">
          {records.map((r) => (
            <div key={r.id} className="p-5 border border-border rounded-md bg-card/60 hover:border-primary/40 transition-colors" data-testid={`service-row-${r.id}`}>
              <div className="flex items-start justify-between gap-4">
                <div className="flex gap-4 flex-1 min-w-0">
                  <div className="p-2.5 rounded-md bg-primary/10 text-primary shrink-0"><WrenchIcon size={20} weight="duotone" /></div>
                  <div className="min-w-0 flex-1">
                    <div className="font-semibold truncate">{r.description}</div>
                    <div className="text-xs text-muted-foreground mt-0.5">{r.date} · {r.odometer?.toLocaleString()} km · {r.center || "—"}</div>
                    {r.notes && <div className="text-sm text-muted-foreground mt-2">{r.notes}</div>}
                  </div>
                </div>
                <div className="text-right shrink-0">
                  <div className="font-display text-lg font-bold number-tabular">₹{r.cost}</div>
                  <button onClick={() => del(r.id)} data-testid={`service-delete-${r.id}`} className="mt-2 p-1 rounded-md text-muted-foreground hover:text-destructive"><TrashIcon size={14} /></button>
                </div>
              </div>
            </div>
          ))}
        </div>
      )}
    </PageContainer>
  );
}
