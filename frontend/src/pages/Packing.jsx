import React, { useEffect, useMemo, useState } from "react";
import client, { formatApiError } from "../lib/api";
import { PageContainer, PageHeader, EmptyState } from "../components/Shared";
import { Button } from "../components/ui/button";
import { Input } from "../components/ui/input";
import { Label } from "../components/ui/label";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "../components/ui/select";
import { Progress } from "../components/ui/progress";
import { Checkbox } from "../components/ui/checkbox";
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger, DialogFooter } from "../components/ui/dialog";
import { PlusIcon, TrashIcon } from "@phosphor-icons/react";
import { toast } from "sonner";

const CATS = ["Riding gear", "Documents", "Electronics", "Tools", "Other"];

export default function Packing() {
  const [items, setItems] = useState([]);
  const [open, setOpen] = useState(false);
  const [form, setForm] = useState({ name: "", category: "Riding gear" });

  const load = () => client.get("/packing").then((r) => setItems(r.data)).catch(() => {});
  useEffect(() => { load(); }, []);

  const add = async () => {
    if (!form.name.trim()) return;
    try { await client.post("/packing", form); toast.success("Item added"); setOpen(false); setForm({ name: "", category: form.category }); load(); }
    catch (e) { toast.error(formatApiError(e)); }
  };

  const toggle = async (item) => {
    try { await client.put(`/packing/${item.id}`, { packed: !item.packed }); load(); }
    catch (e) { toast.error(formatApiError(e)); }
  };

  const del = async (id) => { await client.delete(`/packing/${id}`); load(); };

  const packedCount = items.filter((i) => i.packed).length;
  const pct = items.length ? Math.round((packedCount / items.length) * 100) : 0;
  const byCat = useMemo(() => CATS.reduce((acc, c) => { acc[c] = items.filter((i) => i.category === c); return acc; }, {}), [items]);

  return (
    <PageContainer>
      <PageHeader
        overline="Packing Checklist"
        title="Panniers, sorted."
        description="Riding gear, documents, electronics — track what's packed and what's still on the shelf."
        right={
          <Dialog open={open} onOpenChange={setOpen}>
            <DialogTrigger asChild>
              <Button className="bg-primary hover:bg-primary/90" data-testid="add-pack-button"><PlusIcon size={16} className="mr-1.5" /> Add item</Button>
            </DialogTrigger>
            <DialogContent className="max-w-sm">
              <DialogHeader><DialogTitle className="font-display">Add item</DialogTitle></DialogHeader>
              <div className="space-y-3">
                <div><Label>Item</Label><Input value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value })} data-testid="pack-form-name" placeholder="Riding jacket" /></div>
                <div>
                  <Label>Category</Label>
                  <Select value={form.category} onValueChange={(v) => setForm({ ...form, category: v })}>
                    <SelectTrigger data-testid="pack-form-category"><SelectValue /></SelectTrigger>
                    <SelectContent>{CATS.map((c) => <SelectItem key={c} value={c}>{c}</SelectItem>)}</SelectContent>
                  </Select>
                </div>
              </div>
              <DialogFooter>
                <Button variant="outline" onClick={() => setOpen(false)}>Cancel</Button>
                <Button onClick={add} className="bg-primary hover:bg-primary/90" data-testid="pack-form-save">Add</Button>
              </DialogFooter>
            </DialogContent>
          </Dialog>
        }
      />

      <div className="p-5 border border-border rounded-md bg-card/60 mb-6">
        <div className="flex items-center justify-between mb-3">
          <div className="overline">Progress</div>
          <div className="text-sm font-semibold number-tabular">{packedCount} of {items.length} packed · {pct}%</div>
        </div>
        <Progress value={pct} className="h-2" data-testid="pack-progress" />
      </div>

      {items.length === 0 ? (
        <EmptyState title="Nothing on the checklist yet" description="Add helmets, gloves, documents and chargers to get ready faster." />
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          {CATS.filter((c) => byCat[c].length).map((c) => (
            <div key={c} className="p-5 border border-border rounded-md bg-card/60">
              <div className="overline mb-3">{c}</div>
              <ul className="space-y-2">
                {byCat[c].map((it) => (
                  <li key={it.id} className="flex items-center gap-3 group" data-testid={`pack-item-${it.id}`}>
                    <Checkbox checked={it.packed} onCheckedChange={() => toggle(it)} id={`ck-${it.id}`} data-testid={`pack-toggle-${it.id}`} />
                    <label htmlFor={`ck-${it.id}`} className={`flex-1 text-sm cursor-pointer ${it.packed ? "line-through text-muted-foreground" : ""}`}>{it.name}</label>
                    <button onClick={() => del(it.id)} data-testid={`pack-delete-${it.id}`} className="opacity-0 group-hover:opacity-100 text-muted-foreground hover:text-destructive p-1 transition-opacity"><TrashIcon size={14} /></button>
                  </li>
                ))}
              </ul>
            </div>
          ))}
        </div>
      )}
    </PageContainer>
  );
}
