import React, { useEffect, useState } from "react";
import client, { formatApiError } from "../lib/api";
import { PageContainer, PageHeader, EmptyState } from "../components/Shared";
import { Button } from "../components/ui/button";
import { Input } from "../components/ui/input";
import { Label } from "../components/ui/label";
import { Textarea } from "../components/ui/textarea";
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger, DialogFooter } from "../components/ui/dialog";
import { PlusIcon, TrashIcon, NotebookIcon } from "@phosphor-icons/react";
import { toast } from "sonner";

const EMPTY = { date: new Date().toISOString().slice(0, 10), title: "", location: "", weather: "", companions: "", mood: "", distance_km: 0, duration_min: 0, notes: "" };

export default function Journal() {
  const [entries, setEntries] = useState([]);
  const [open, setOpen] = useState(false);
  const [form, setForm] = useState(EMPTY);
  const [saving, setSaving] = useState(false);

  const load = () => client.get("/journal").then((r) => setEntries(r.data)).catch(() => {});
  useEffect(() => { load(); }, []);

  const setField = (k) => (e) => setForm({ ...form, [k]: e?.target ? e.target.value : e });

  const save = async () => {
    setSaving(true);
    try {
      await client.post("/journal", { ...form, distance_km: Number(form.distance_km), duration_min: Number(form.duration_min) });
      toast.success("Journal entry saved"); setOpen(false); setForm(EMPTY); load();
    } catch (e) { toast.error(formatApiError(e)); }
    finally { setSaving(false); }
  };

  const del = async (id) => { await client.delete(`/journal/${id}`); toast.success("Deleted"); load(); };

  return (
    <PageContainer>
      <PageHeader
        overline="Ride Journal"
        title="Preserve every ride worth remembering."
        description="Where, when, with whom, in what mood — your rides, as they happened."
        right={
          <Dialog open={open} onOpenChange={setOpen}>
            <DialogTrigger asChild>
              <Button className="bg-primary hover:bg-primary/90" data-testid="add-journal-button"><PlusIcon size={16} className="mr-1.5" /> New entry</Button>
            </DialogTrigger>
            <DialogContent className="max-w-lg">
              <DialogHeader><DialogTitle className="font-display">New journal entry</DialogTitle></DialogHeader>
              <div className="grid grid-cols-2 gap-3">
                <div className="col-span-2"><Label>Title</Label><Input value={form.title} onChange={setField("title")} data-testid="journal-form-title" placeholder="Through the Atal tunnel" /></div>
                <div><Label>Date</Label><Input type="date" value={form.date} onChange={setField("date")} data-testid="journal-form-date" /></div>
                <div><Label>Location</Label><Input value={form.location} onChange={setField("location")} data-testid="journal-form-location" placeholder="Sissu, HP" /></div>
                <div><Label>Weather</Label><Input value={form.weather} onChange={setField("weather")} data-testid="journal-form-weather" placeholder="14° · overcast" /></div>
                <div><Label>Mood</Label><Input value={form.mood} onChange={setField("mood")} data-testid="journal-form-mood" placeholder="Great" /></div>
                <div><Label>Companions</Label><Input value={form.companions} onChange={setField("companions")} data-testid="journal-form-companions" placeholder="With brother" /></div>
                <div><Label>Distance (km)</Label><Input type="number" value={form.distance_km} onChange={setField("distance_km")} data-testid="journal-form-distance" /></div>
                <div><Label>Duration (min)</Label><Input type="number" value={form.duration_min} onChange={setField("duration_min")} data-testid="journal-form-duration" /></div>
                <div className="col-span-2"><Label>Notes</Label><Textarea rows={4} value={form.notes} onChange={setField("notes")} data-testid="journal-form-notes" placeholder="Cold at the top, but the descent…" /></div>
              </div>
              <DialogFooter>
                <Button variant="outline" onClick={() => setOpen(false)}>Cancel</Button>
                <Button disabled={saving} onClick={save} className="bg-primary hover:bg-primary/90" data-testid="journal-form-save">
                  {saving ? "Saving…" : "Save entry"}
                </Button>
              </DialogFooter>
            </DialogContent>
          </Dialog>
        }
      />

      {entries.length === 0 ? (
        <EmptyState title="Your journal is empty" description="Every road has a story. Start with your last one." />
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          {entries.map((j) => (
            <div key={j.id} className="p-6 border border-border rounded-md bg-card/60 hover:border-primary/40 transition-colors" data-testid={`journal-card-${j.id}`}>
              <div className="flex items-start justify-between">
                <div className="flex-1 min-w-0">
                  <div className="flex items-center gap-2 mb-2">
                    <NotebookIcon size={18} weight="duotone" className="text-primary" />
                    <div className="overline text-[10px]">{j.date} · {j.location || "—"}</div>
                  </div>
                  <div className="font-display text-xl font-bold">{j.title}</div>
                </div>
                <button onClick={() => del(j.id)} data-testid={`journal-delete-${j.id}`} className="p-1.5 rounded-md text-muted-foreground hover:text-destructive"><TrashIcon size={14} /></button>
              </div>
              <div className="flex flex-wrap gap-2 mt-3 text-[10px]">
                {j.weather && <span className="px-2 py-0.5 border border-border rounded-full">{j.weather}</span>}
                {j.mood && <span className="px-2 py-0.5 border border-border rounded-full">{j.mood}</span>}
                {j.companions && <span className="px-2 py-0.5 border border-border rounded-full">{j.companions}</span>}
                {j.distance_km > 0 && <span className="px-2 py-0.5 border border-border rounded-full number-tabular">{j.distance_km} km</span>}
                {j.duration_min > 0 && <span className="px-2 py-0.5 border border-border rounded-full number-tabular">{Math.floor(j.duration_min / 60)}h {j.duration_min % 60}m</span>}
              </div>
              {j.notes && <p className="text-sm text-muted-foreground mt-4 leading-relaxed line-clamp-4">{j.notes}</p>}
            </div>
          ))}
        </div>
      )}
    </PageContainer>
  );
}
