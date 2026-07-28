import React, { useEffect, useState } from "react";
import client, { formatApiError } from "../lib/api";
import { PageContainer, PageHeader, EmptyState } from "../components/Shared";
import { Button } from "../components/ui/button";
import { Input } from "../components/ui/input";
import { Label } from "../components/ui/label";
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger, DialogFooter } from "../components/ui/dialog";
import { Switch } from "../components/ui/switch";
import { MotorcycleIcon, PlusIcon, TrashIcon } from "@phosphor-icons/react";
import { toast } from "sonner";

const EMPTY = { name: "", make: "", model: "", year: 2023, cc: 350, reg_no: "", odometer: 0, fuel_type: "Petrol", avg_mileage: 0, insurance_valid_until: "", puc_valid_until: "", is_primary: false };

export default function Garage() {
  const [bikes, setBikes] = useState([]);
  const [open, setOpen] = useState(false);
  const [form, setForm] = useState(EMPTY);
  const [saving, setSaving] = useState(false);

  const load = () => client.get("/bikes").then((r) => setBikes(r.data)).catch(() => {});
  useEffect(() => { load(); }, []);

  const setField = (k) => (e) => setForm({ ...form, [k]: e?.target ? e.target.value : e });

  const save = async () => {
    setSaving(true);
    try {
      await client.post("/bikes", {
        ...form,
        year: Number(form.year), cc: Number(form.cc), odometer: Number(form.odometer), avg_mileage: Number(form.avg_mileage),
      });
      toast.success("Bike added");
      setOpen(false); setForm(EMPTY); load();
    } catch (e) { toast.error(formatApiError(e)); }
    finally { setSaving(false); }
  };

  const del = async (id) => {
    if (!window.confirm("Remove this bike?")) return;
    try { await client.delete(`/bikes/${id}`); toast.success("Bike removed"); load(); }
    catch (e) { toast.error(formatApiError(e)); }
  };

  return (
    <PageContainer>
      <PageHeader
        overline="My Garage"
        title="Every bike, one place."
        description="Track make, model, mileage, insurance, and service intervals for every motorcycle in your garage."
        right={
          <Dialog open={open} onOpenChange={setOpen}>
            <DialogTrigger asChild>
              <Button className="bg-primary hover:bg-primary/90" data-testid="add-bike-button"><PlusIcon size={16} className="mr-1.5" /> Add bike</Button>
            </DialogTrigger>
            <DialogContent className="max-w-lg">
              <DialogHeader><DialogTitle className="font-display">Add a motorcycle</DialogTitle></DialogHeader>
              <div className="grid grid-cols-2 gap-4">
                <div className="col-span-2"><Label>Nickname</Label><Input value={form.name} onChange={setField("name")} data-testid="bike-form-name" placeholder="Classic 350" /></div>
                <div><Label>Make</Label><Input value={form.make} onChange={setField("make")} data-testid="bike-form-make" placeholder="Royal Enfield" /></div>
                <div><Label>Model</Label><Input value={form.model} onChange={setField("model")} data-testid="bike-form-model" placeholder="Classic 350" /></div>
                <div><Label>Year</Label><Input type="number" value={form.year} onChange={setField("year")} data-testid="bike-form-year" /></div>
                <div><Label>Engine (cc)</Label><Input type="number" value={form.cc} onChange={setField("cc")} data-testid="bike-form-cc" /></div>
                <div><Label>Reg. no.</Label><Input value={form.reg_no} onChange={setField("reg_no")} data-testid="bike-form-reg" placeholder="DL 8S 4471" /></div>
                <div><Label>Odometer (km)</Label><Input type="number" value={form.odometer} onChange={setField("odometer")} data-testid="bike-form-odo" /></div>
                <div><Label>Avg mileage (km/L)</Label><Input type="number" step="0.1" value={form.avg_mileage} onChange={setField("avg_mileage")} data-testid="bike-form-avg" /></div>
                <div><Label>Fuel type</Label><Input value={form.fuel_type} onChange={setField("fuel_type")} data-testid="bike-form-fuel" /></div>
                <div className="col-span-2 flex items-center justify-between border border-border rounded-md px-3 py-2">
                  <div><div className="text-sm font-medium">Set as primary</div><div className="text-xs text-muted-foreground">Shows on your dashboard.</div></div>
                  <Switch checked={form.is_primary} onCheckedChange={(v) => setForm({ ...form, is_primary: v })} data-testid="bike-form-primary" />
                </div>
              </div>
              <DialogFooter>
                <Button variant="outline" onClick={() => setOpen(false)} data-testid="bike-form-cancel">Cancel</Button>
                <Button disabled={saving} onClick={save} className="bg-primary hover:bg-primary/90" data-testid="bike-form-save">
                  {saving ? "Saving…" : "Add bike"}
                </Button>
              </DialogFooter>
            </DialogContent>
          </Dialog>
        }
      />

      {bikes.length === 0 ? (
        <EmptyState
          title="Your garage is empty"
          description="Add your first motorcycle to unlock fuel logs, service tracking and mileage insights."
          action={<Button onClick={() => setOpen(true)} className="bg-primary hover:bg-primary/90" data-testid="empty-add-bike-btn"><PlusIcon size={16} className="mr-1.5" /> Add a bike</Button>}
        />
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          {bikes.map((b) => (
            <div key={b.id} className="p-6 border border-border rounded-md bg-card/60 hover:border-primary/40 transition-colors" data-testid={`bike-card-${b.id}`}>
              <div className="flex items-start justify-between">
                <div className="flex gap-4">
                  <div className="p-3 rounded-md bg-primary/10 text-primary"><MotorcycleIcon size={32} weight="duotone" /></div>
                  <div>
                    <div className="font-display text-xl font-bold flex items-center gap-2">
                      {b.name}
                      {b.is_primary && <span className="text-[10px] bg-primary text-primary-foreground px-2 py-0.5 rounded-full">PRIMARY</span>}
                    </div>
                    <div className="text-sm text-muted-foreground">{b.make} · {b.year} · {b.cc}cc</div>
                  </div>
                </div>
                <button onClick={() => del(b.id)} data-testid={`bike-delete-${b.id}`} className="p-1.5 rounded-md text-muted-foreground hover:text-destructive hover:bg-destructive/10 transition-colors">
                  <TrashIcon size={16} />
                </button>
              </div>
              <div className="grid grid-cols-2 gap-4 mt-5 text-sm">
                <div><div className="overline text-[10px]">Odometer</div><div className="font-semibold mt-1 number-tabular">{b.odometer?.toLocaleString() || 0} km</div></div>
                <div><div className="overline text-[10px]">Reg. no.</div><div className="font-semibold mt-1">{b.reg_no || "—"}</div></div>
                <div><div className="overline text-[10px]">Fuel type</div><div className="font-semibold mt-1">{b.fuel_type}</div></div>
                <div><div className="overline text-[10px]">Avg mileage</div><div className="font-semibold mt-1 number-tabular">{b.avg_mileage} km/L</div></div>
              </div>
            </div>
          ))}
        </div>
      )}
    </PageContainer>
  );
}
