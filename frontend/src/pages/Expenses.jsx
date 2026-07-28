import React, { useEffect, useMemo, useState } from "react";
import client, { formatApiError } from "../lib/api";
import { PageContainer, PageHeader, StatTile, EmptyState } from "../components/Shared";
import { Button } from "../components/ui/button";
import { Input } from "../components/ui/input";
import { Label } from "../components/ui/label";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "../components/ui/select";
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger, DialogFooter } from "../components/ui/dialog";
import { PlusIcon, TrashIcon } from "@phosphor-icons/react";
import { toast } from "sonner";

const CATS = ["Fuel", "Hotel", "Food", "Toll", "Other"];
const EMPTY = { date: new Date().toISOString().slice(0, 10), category: "Fuel", amount: 0, note: "", split_riders: 1, trip_id: "" };

export default function Expenses() {
  const [items, setItems] = useState([]);
  const [trips, setTrips] = useState([]);
  const [open, setOpen] = useState(false);
  const [form, setForm] = useState(EMPTY);
  const [saving, setSaving] = useState(false);
  const [tripFilter, setTripFilter] = useState("all");

  const load = () => Promise.all([client.get("/expenses"), client.get("/trips")]).then(([e, t]) => { setItems(e.data); setTrips(t.data); }).catch(() => {});
  useEffect(() => { load(); }, []);

  const setField = (k) => (e) => setForm({ ...form, [k]: e?.target ? e.target.value : e });

  const save = async () => {
    setSaving(true);
    try {
      const payload = { ...form, amount: Number(form.amount), split_riders: Number(form.split_riders) };
      if (!payload.trip_id) payload.trip_id = null;
      await client.post("/expenses", payload);
      toast.success("Expense added"); setOpen(false); setForm(EMPTY); load();
    } catch (e) { toast.error(formatApiError(e)); }
    finally { setSaving(false); }
  };

  const del = async (id) => { await client.delete(`/expenses/${id}`); toast.success("Deleted"); load(); };

  const filtered = useMemo(() => tripFilter === "all" ? items : items.filter((i) => i.trip_id === tripFilter), [items, tripFilter]);
  const total = filtered.reduce((s, e) => s + (e.amount || 0), 0);
  const selectedTrip = trips.find((t) => t.id === tripFilter);
  const byCat = CATS.reduce((acc, c) => { acc[c] = filtered.filter((e) => e.category === c).reduce((s, e) => s + (e.amount || 0), 0); return acc; }, {});

  return (
    <PageContainer>
      <PageHeader
        overline="Expense Manager"
        title="Budget vs reality, at a glance."
        description="Track spend by category, per trip, and split with fellow riders."
        right={
          <Dialog open={open} onOpenChange={setOpen}>
            <DialogTrigger asChild>
              <Button className="bg-primary hover:bg-primary/90" data-testid="add-expense-button"><PlusIcon size={16} className="mr-1.5" /> Add expense</Button>
            </DialogTrigger>
            <DialogContent className="max-w-md">
              <DialogHeader><DialogTitle className="font-display">Add expense</DialogTitle></DialogHeader>
              <div className="space-y-3">
                <div><Label>Date</Label><Input type="date" value={form.date} onChange={setField("date")} data-testid="expense-form-date" /></div>
                <div>
                  <Label>Category</Label>
                  <Select value={form.category} onValueChange={(v) => setForm({ ...form, category: v })}>
                    <SelectTrigger data-testid="expense-form-category"><SelectValue /></SelectTrigger>
                    <SelectContent>{CATS.map((c) => <SelectItem key={c} value={c}>{c}</SelectItem>)}</SelectContent>
                  </Select>
                </div>
                <div><Label>Amount (₹)</Label><Input type="number" value={form.amount} onChange={setField("amount")} data-testid="expense-form-amount" /></div>
                <div>
                  <Label>Trip (optional)</Label>
                  <Select value={form.trip_id || "none"} onValueChange={(v) => setForm({ ...form, trip_id: v === "none" ? "" : v })}>
                    <SelectTrigger data-testid="expense-form-trip"><SelectValue placeholder="None" /></SelectTrigger>
                    <SelectContent>
                      <SelectItem value="none">None</SelectItem>
                      {trips.map((t) => <SelectItem key={t.id} value={t.id}>{t.title}</SelectItem>)}
                    </SelectContent>
                  </Select>
                </div>
                <div><Label>Note</Label><Input value={form.note} onChange={setField("note")} data-testid="expense-form-note" /></div>
                <div><Label>Split with riders</Label><Input type="number" min="1" value={form.split_riders} onChange={setField("split_riders")} data-testid="expense-form-split" /></div>
              </div>
              <DialogFooter>
                <Button variant="outline" onClick={() => setOpen(false)}>Cancel</Button>
                <Button disabled={saving} onClick={save} className="bg-primary hover:bg-primary/90" data-testid="expense-form-save">
                  {saving ? "Saving…" : "Save"}
                </Button>
              </DialogFooter>
            </DialogContent>
          </Dialog>
        }
      />

      <div className="flex items-center gap-3 mb-6">
        <Label className="text-xs text-muted-foreground">Filter by trip:</Label>
        <Select value={tripFilter} onValueChange={setTripFilter}>
          <SelectTrigger className="w-64" data-testid="expense-trip-filter"><SelectValue /></SelectTrigger>
          <SelectContent>
            <SelectItem value="all">All trips</SelectItem>
            {trips.map((t) => <SelectItem key={t.id} value={t.id}>{t.title}</SelectItem>)}
          </SelectContent>
        </Select>
      </div>

      <div className="grid grid-cols-2 md:grid-cols-6 gap-3 mb-8">
        <div className="col-span-2 p-5 rounded-md border border-primary/40 bg-primary/5">
          <div className="overline text-primary">{selectedTrip ? selectedTrip.title : "Total spent"}</div>
          <div className="font-display text-3xl font-bold mt-2 number-tabular">₹{total.toLocaleString()}</div>
          {selectedTrip && <div className="text-xs text-muted-foreground mt-1">of ₹{selectedTrip.budget} budget · {Math.round((total / (selectedTrip.budget || 1)) * 100)}%</div>}
        </div>
        {CATS.map((c) => <StatTile key={c} label={c} value={`₹${byCat[c].toLocaleString()}`} testid={`expense-cat-${c.toLowerCase()}`} />)}
      </div>

      {filtered.length === 0 ? (
        <EmptyState title="No expenses logged" description="Add fuel, hotels, food and tolls as you go." />
      ) : (
        <div className="border border-border rounded-md divide-y divide-border overflow-hidden">
          {filtered.map((e) => (
            <div key={e.id} className="grid grid-cols-12 gap-3 p-4 items-center hover:bg-muted/30" data-testid={`expense-row-${e.id}`}>
              <div className="col-span-3 text-xs text-muted-foreground">{e.date}</div>
              <div className="col-span-2"><span className="inline-block px-2 py-0.5 text-[10px] rounded-full border border-border">{e.category}</span></div>
              <div className="col-span-4 text-sm truncate">{e.note || "—"}</div>
              <div className="col-span-2 text-right font-semibold number-tabular">₹{e.amount}</div>
              <div className="col-span-1 text-right">
                <button onClick={() => del(e.id)} data-testid={`expense-delete-${e.id}`} className="p-1 text-muted-foreground hover:text-destructive"><TrashIcon size={14} /></button>
              </div>
            </div>
          ))}
        </div>
      )}
    </PageContainer>
  );
}
