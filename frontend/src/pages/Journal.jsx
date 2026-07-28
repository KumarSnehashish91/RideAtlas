import React, { useEffect, useRef, useState } from "react";
import { Link } from "react-router-dom";
import client, { formatApiError, API } from "../lib/api";
import { PageContainer, PageHeader, EmptyState } from "../components/Shared";
import { Button } from "../components/ui/button";
import { Input } from "../components/ui/input";
import { Label } from "../components/ui/label";
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger, DialogFooter, DialogDescription } from "../components/ui/dialog";
import { PlusIcon, TrashIcon, NotebookIcon, SparkleIcon, ImageIcon, XCircleIcon } from "@phosphor-icons/react";
import { toast } from "sonner";
import axios from "axios";

const MOODS = ["Great", "Good", "Reflective", "Tired", "Exhilarated", "Peaceful"];

export default function Journal() {
  const [entries, setEntries] = useState([]);
  const [open, setOpen] = useState(false);
  const [mood, setMood] = useState("Great");
  const [duration, setDuration] = useState(120);
  const [note, setNote] = useState("");
  const [files, setFiles] = useState([]);
  const [previews, setPreviews] = useState([]);
  const [generating, setGenerating] = useState(false);
  const fileRef = useRef();

  const load = () => client.get("/journal").then((r) => setEntries(r.data)).catch(() => {});
  useEffect(() => { load(); }, []);

  const resetForm = () => {
    setMood("Great"); setDuration(120); setNote("");
    setFiles([]); setPreviews([]);
    if (fileRef.current) fileRef.current.value = "";
  };

  const onFiles = (e) => {
    const list = Array.from(e.target.files || []).slice(0, 3);
    if (list.some((f) => f.size > 6 * 1024 * 1024)) {
      toast.error("Each photo must be under 6 MB"); return;
    }
    setFiles(list);
    setPreviews(list.map((f) => URL.createObjectURL(f)));
  };

  const removeFile = (idx) => {
    const nf = files.filter((_, i) => i !== idx);
    setFiles(nf);
    setPreviews(nf.map((f) => URL.createObjectURL(f)));
    if (fileRef.current) fileRef.current.value = "";
  };

  const generate = async () => {
    if (!files.length) { toast.error("Add at least one photo"); return; }
    setGenerating(true);
    try {
      const fd = new FormData();
      fd.append("mood", mood);
      fd.append("duration_min", String(duration));
      fd.append("note", note);
      files.forEach((f) => fd.append("photos", f));
      const token = localStorage.getItem("ra_token");
      await axios.post(`${API}/journal/generate`, fd, {
        withCredentials: true,
        headers: token ? { Authorization: `Bearer ${token}` } : {},
      });
      toast.success("Journal entry generated");
      setOpen(false); resetForm(); load();
    } catch (e) {
      toast.error(formatApiError(e));
    } finally { setGenerating(false); }
  };

  const del = async (id) => { await client.delete(`/journal/${id}`); toast.success("Deleted"); load(); };

  return (
    <PageContainer>
      <PageHeader
        overline="Ride Journal · AI-powered"
        title="Snap the moment. We'll tell the story."
        description="Upload 1–3 photos from your ride, jot the mood and duration — Gemini vision writes a rich narrative and fills in the details."
        right={
          <Dialog open={open} onOpenChange={(v) => { setOpen(v); if (!v) resetForm(); }}>
            <DialogTrigger asChild>
              <Button className="bg-primary hover:bg-primary/90" data-testid="add-journal-button">
                <SparkleIcon size={16} weight="fill" className="mr-1.5" /> New AI entry
              </Button>
            </DialogTrigger>
            <DialogContent className="max-w-lg">
              <DialogHeader>
                <DialogTitle className="font-display flex items-center gap-2">
                  <SparkleIcon size={20} weight="fill" className="text-primary" />
                  AI journal entry
                </DialogTitle>
                <DialogDescription>Add photos and a couple of quick notes — the AI fills in the rest.</DialogDescription>
              </DialogHeader>

              <div className="space-y-4">
                {/* Photos */}
                <div>
                  <Label>Photos (1–3)</Label>
                  <div className="grid grid-cols-3 gap-2 mt-2">
                    {previews.map((src, i) => (
                      <div key={i} className="relative aspect-square border border-border rounded-md overflow-hidden" data-testid={`journal-photo-preview-${i}`}>
                        <img src={src} alt="" className="w-full h-full object-cover" />
                        <button type="button" onClick={() => removeFile(i)} className="absolute top-1 right-1 bg-background/80 rounded-full p-0.5 text-muted-foreground hover:text-destructive">
                          <XCircleIcon size={18} weight="fill" />
                        </button>
                      </div>
                    ))}
                    {previews.length < 3 && (
                      <label className="aspect-square border border-dashed border-border rounded-md flex flex-col items-center justify-center gap-1 text-muted-foreground hover:border-primary hover:text-primary transition-colors cursor-pointer" data-testid="journal-add-photos-btn">
                        <ImageIcon size={22} weight="duotone" />
                        <span className="text-[10px] text-center px-1">Add photo</span>
                        <input ref={fileRef} type="file" accept="image/jpeg,image/png,image/webp" multiple hidden onChange={onFiles} data-testid="journal-photos-input" />
                      </label>
                    )}
                  </div>
                </div>

                {/* Mood */}
                <div>
                  <Label>Mood</Label>
                  <div className="flex flex-wrap gap-1.5 mt-2">
                    {MOODS.map((m) => (
                      <button
                        key={m}
                        type="button"
                        onClick={() => setMood(m)}
                        data-testid={`journal-mood-${m.toLowerCase()}`}
                        className={`px-3 py-1.5 rounded-full text-xs border transition-colors ${
                          mood === m ? "border-primary bg-primary/10 text-primary font-medium" : "border-border text-muted-foreground hover:border-primary/40"
                        }`}
                      >
                        {m}
                      </button>
                    ))}
                  </div>
                </div>

                {/* Duration */}
                <div>
                  <Label htmlFor="dur">Duration (minutes)</Label>
                  <Input id="dur" type="number" value={duration} onChange={(e) => setDuration(Number(e.target.value))} data-testid="journal-duration-input" />
                </div>

                {/* Note */}
                <div>
                  <Label htmlFor="note">One-line note (optional)</Label>
                  <Input id="note" value={note} onChange={(e) => setNote(e.target.value)} placeholder="Twisty ghat road, chai stop near river" data-testid="journal-note-input" />
                </div>
              </div>

              <DialogFooter>
                <Button variant="outline" onClick={() => setOpen(false)}>Cancel</Button>
                <Button onClick={generate} disabled={generating || !files.length} className="bg-primary hover:bg-primary/90" data-testid="journal-generate-button">
                  {generating ? (
                    <><span className="mr-2 inline-block h-3 w-3 rounded-full border-2 border-white/60 border-t-transparent animate-spin" /> Generating…</>
                  ) : (
                    <><SparkleIcon size={14} weight="fill" className="mr-1.5" /> Generate entry</>
                  )}
                </Button>
              </DialogFooter>
            </DialogContent>
          </Dialog>
        }
      />

      {entries.length === 0 ? (
        <EmptyState
          title="Your journal is empty"
          description="Upload a photo from your last ride — the AI writes the story for you."
        />
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          {entries.map((j) => (
            <div key={j.id} className="relative border border-border rounded-md bg-card/60 hover:border-primary/40 transition-colors overflow-hidden group" data-testid={`journal-card-${j.id}`}>
              <button
                onClick={(e) => { e.preventDefault(); e.stopPropagation(); del(j.id); }}
                data-testid={`journal-delete-${j.id}`}
                aria-label="Delete entry"
                className="absolute top-3 right-3 z-10 p-1.5 rounded-md bg-background/80 backdrop-blur text-muted-foreground hover:text-destructive opacity-0 group-hover:opacity-100 transition-opacity"
              >
                <TrashIcon size={14} />
              </button>
              <Link to={`/journal/${j.id}`} className="block" data-testid={`journal-open-${j.id}`}>
                {j.photos && j.photos.length > 0 && (
                  <div className={`grid ${j.photos.length === 1 ? "grid-cols-1" : j.photos.length === 2 ? "grid-cols-2" : "grid-cols-3"} gap-0.5 bg-border overflow-hidden`}>
                    {j.photos.slice(0, 3).map((src, i) => (
                      <img key={i} src={src} alt="" className="w-full h-40 md:h-48 object-cover group-hover:scale-[1.02] transition-transform duration-300" />
                    ))}
                  </div>
                )}
                <div className="p-5">
                  <div className="flex items-center gap-2 mb-2 flex-wrap">
                    <NotebookIcon size={16} weight="duotone" className="text-primary" />
                    <div className="overline text-[10px]">{j.date} · {j.location || "—"}</div>
                    {j.ai_generated && (
                      <span className="inline-flex items-center gap-1 text-[9px] px-1.5 py-0.5 rounded-full bg-primary/10 text-primary border border-primary/30">
                        <SparkleIcon size={10} weight="fill" /> AI
                      </span>
                    )}
                  </div>
                  <div className="font-display text-xl font-bold leading-tight group-hover:text-primary transition-colors">{j.title}</div>
                  <div className="flex flex-wrap gap-1.5 mt-3 text-[10px]">
                    {j.weather && <span className="px-2 py-0.5 border border-border rounded-full">{j.weather}</span>}
                    {j.mood && <span className="px-2 py-0.5 border border-border rounded-full">{j.mood}</span>}
                    {j.companions && <span className="px-2 py-0.5 border border-border rounded-full">{j.companions}</span>}
                    {j.distance_km > 0 && <span className="px-2 py-0.5 border border-border rounded-full number-tabular">{j.distance_km} km</span>}
                    {j.duration_min > 0 && <span className="px-2 py-0.5 border border-border rounded-full number-tabular">{Math.floor(j.duration_min / 60)}h {j.duration_min % 60}m</span>}
                  </div>
                  {j.notes && <p className="text-sm text-muted-foreground mt-4 leading-relaxed line-clamp-3">{j.notes}</p>}
                  <div className="mt-4 text-xs text-primary font-medium opacity-0 group-hover:opacity-100 transition-opacity">Read full entry →</div>
                </div>
              </Link>
            </div>
          ))}
        </div>
      )}
    </PageContainer>
  );
}
