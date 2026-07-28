import React, { useEffect, useState } from "react";
import { useParams, useNavigate, Link } from "react-router-dom";
import client, { formatApiError } from "../lib/api";
import { Button } from "../components/ui/button";
import {
  ArrowLeftIcon, TrashIcon, SparkleIcon, CalendarBlankIcon, MapPinIcon,
  CloudIcon, UsersThreeIcon, HeartIcon, ClockIcon, PathIcon, CompassIcon,
} from "@phosphor-icons/react";
import { toast } from "sonner";

function readingTime(text) {
  const words = (text || "").trim().split(/\s+/).length;
  return Math.max(1, Math.round(words / 200));
}

function MetaRow({ Icon, label, value }) {
  if (!value) return null;
  return (
    <div className="flex items-center gap-2 text-sm">
      <Icon size={16} weight="duotone" className="text-primary shrink-0" />
      <span className="text-muted-foreground">{label}</span>
      <span className="font-medium">{value}</span>
    </div>
  );
}

export default function JournalDetail() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [entry, setEntry] = useState(null);
  const [notFound, setNotFound] = useState(false);
  const [lightbox, setLightbox] = useState(null);

  useEffect(() => {
    let mounted = true;
    client.get(`/journal/${id}`)
      .then((r) => { if (mounted) setEntry(r.data); })
      .catch(() => { if (mounted) setNotFound(true); });
    return () => { mounted = false; };
  }, [id]);

  const del = async () => {
    if (!window.confirm("Delete this journal entry? This cannot be undone.")) return;
    try {
      await client.delete(`/journal/${id}`);
      toast.success("Entry deleted");
      navigate("/journal");
    } catch (e) { toast.error(formatApiError(e)); }
  };

  if (notFound) {
    return (
      <div className="min-h-screen flex items-center justify-center text-center p-8">
        <div>
          <div className="overline mb-3">404 · Off route</div>
          <h1 className="font-display text-3xl font-bold">This ride doesn't exist.</h1>
          <p className="text-muted-foreground mt-2">The entry may have been deleted or the link is wrong.</p>
          <Button asChild className="mt-6 bg-primary hover:bg-primary/90"><Link to="/journal">Back to Journal</Link></Button>
        </div>
      </div>
    );
  }

  if (!entry) {
    return <div className="p-10 text-muted-foreground">Loading ride…</div>;
  }

  const hero = entry.photos?.[0];
  const gallery = (entry.photos || []).slice(1);
  const mins = readingTime(entry.notes);
  const dateFmt = entry.date
    ? new Date(entry.date).toLocaleDateString("en-US", { weekday: "long", year: "numeric", month: "long", day: "numeric" })
    : "";

  // Split narrative into paragraphs for typography
  const paragraphs = (entry.notes || "").split(/\n\n+|(?<=[.!?])\s{2,}/).map((p) => p.trim()).filter(Boolean);
  const paragraphChunks = paragraphs.length > 1
    ? paragraphs
    : (entry.notes || "").split(/(?<=[.!?])\s+/).reduce((acc, s, i) => {
        const idx = Math.floor(i / 3);
        acc[idx] = (acc[idx] ? acc[idx] + " " : "") + s;
        return acc;
      }, []).filter(Boolean);

  return (
    <article className="min-h-screen bg-background">
      {/* Sticky top nav bar */}
      <div className="sticky top-0 z-40 backdrop-blur-md bg-background/70 border-b border-border/60">
        <div className="max-w-4xl mx-auto px-6 h-14 flex items-center justify-between">
          <Link to="/journal" data-testid="journal-detail-back" className="flex items-center gap-2 text-sm text-muted-foreground hover:text-foreground transition-colors">
            <ArrowLeftIcon size={16} /> All entries
          </Link>
          <div className="flex items-center gap-2">
            <CompassIcon size={18} weight="duotone" className="text-primary" />
            <span className="font-display text-sm font-bold">RideAtlas Journal</span>
          </div>
          <button
            onClick={del}
            data-testid="journal-detail-delete"
            className="flex items-center gap-1.5 text-sm text-muted-foreground hover:text-destructive transition-colors"
          >
            <TrashIcon size={14} /> Delete
          </button>
        </div>
      </div>

      {/* Hero */}
      {hero && (
        <div className="relative w-full h-[52vh] md:h-[68vh] overflow-hidden">
          <img src={hero} alt={entry.title} className="w-full h-full object-cover" data-testid="journal-detail-hero-image" />
          <div className="absolute inset-0 bg-gradient-to-t from-background via-background/40 to-transparent" />
          <div className="absolute inset-x-0 bottom-0 pb-12 md:pb-16">
            <div className="max-w-4xl mx-auto px-6">
              <div className="flex items-center gap-2 mb-4">
                {entry.ai_generated && (
                  <span className="inline-flex items-center gap-1 text-[10px] px-2 py-0.5 rounded-full bg-primary/20 text-primary border border-primary/40 backdrop-blur-sm">
                    <SparkleIcon size={11} weight="fill" /> AI-narrated
                  </span>
                )}
                {entry.location && (
                  <span className="text-xs text-muted-foreground flex items-center gap-1">
                    <MapPinIcon size={12} weight="duotone" /> {entry.location}
                  </span>
                )}
              </div>
              <h1 className="font-display text-4xl md:text-6xl lg:text-7xl font-extrabold tracking-tighter leading-[0.95] max-w-3xl" data-testid="journal-detail-title">
                {entry.title}
              </h1>
              <div className="flex flex-wrap items-center gap-x-6 gap-y-2 mt-6 text-sm text-muted-foreground">
                <span className="flex items-center gap-1.5"><CalendarBlankIcon size={14} weight="duotone" /> {dateFmt}</span>
                <span className="flex items-center gap-1.5"><ClockIcon size={14} weight="duotone" /> {mins} min read</span>
                {entry.distance_km > 0 && <span className="flex items-center gap-1.5 number-tabular"><PathIcon size={14} weight="duotone" /> {entry.distance_km} km</span>}
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Body */}
      <div className="max-w-4xl mx-auto px-6 py-12 md:py-16 grid md:grid-cols-12 gap-10">
        {/* Left rail — meta */}
        <aside className="md:col-span-3 md:sticky md:top-20 md:self-start">
          <div className="overline mb-4">Ride notes</div>
          <div className="space-y-3">
            <MetaRow Icon={MapPinIcon} label="Where" value={entry.location} />
            <MetaRow Icon={CloudIcon} label="Weather" value={entry.weather} />
            <MetaRow Icon={UsersThreeIcon} label="Company" value={entry.companions} />
            <MetaRow Icon={HeartIcon} label="Mood" value={entry.mood} />
            <MetaRow Icon={PathIcon} label="Distance" value={entry.distance_km > 0 ? `${entry.distance_km} km` : null} />
            <MetaRow
              Icon={ClockIcon}
              label="Duration"
              value={entry.duration_min > 0 ? `${Math.floor(entry.duration_min / 60)}h ${entry.duration_min % 60}m` : null}
            />
          </div>
        </aside>

        {/* Article */}
        <div className="md:col-span-9">
          {!hero && (
            <>
              {entry.ai_generated && (
                <span className="inline-flex items-center gap-1 text-[10px] px-2 py-0.5 rounded-full bg-primary/10 text-primary border border-primary/30 mb-4">
                  <SparkleIcon size={11} weight="fill" /> AI-narrated
                </span>
              )}
              <h1 className="font-display text-4xl md:text-5xl font-extrabold tracking-tighter leading-[1]">{entry.title}</h1>
              <div className="flex flex-wrap items-center gap-x-6 gap-y-2 mt-4 text-sm text-muted-foreground">
                <span className="flex items-center gap-1.5"><CalendarBlankIcon size={14} weight="duotone" /> {dateFmt}</span>
                <span className="flex items-center gap-1.5"><ClockIcon size={14} weight="duotone" /> {mins} min read</span>
              </div>
              <div className="h-8" />
            </>
          )}

          {paragraphChunks.map((para, i) => (
            <p
              key={i}
              className={`font-body text-lg md:text-xl leading-[1.75] text-foreground/90 mb-6 ${
                i === 0 ? "first-letter:font-display first-letter:text-6xl first-letter:font-bold first-letter:text-primary first-letter:float-left first-letter:leading-none first-letter:pr-3 first-letter:pt-1" : ""
              }`}
              data-testid={`journal-detail-paragraph-${i}`}
            >
              {para}
            </p>
          ))}

          {/* Inline gallery */}
          {gallery.length > 0 && (
            <div className="mt-10">
              <div className="overline mb-4">More from this ride</div>
              <div className={`grid gap-3 ${gallery.length === 1 ? "grid-cols-1" : "grid-cols-2"}`}>
                {gallery.map((src, i) => (
                  <button
                    key={i}
                    onClick={() => setLightbox(src)}
                    className="group relative overflow-hidden rounded-md border border-border aspect-[4/3]"
                    data-testid={`journal-detail-gallery-${i}`}
                  >
                    <img src={src} alt="" className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-500" />
                  </button>
                ))}
              </div>
            </div>
          )}

          {/* Signature */}
          <div className="mt-16 pt-8 border-t border-border">
            <div className="flex items-center gap-3">
              <div className="p-2.5 rounded-md bg-primary/10 text-primary"><CompassIcon size={20} weight="duotone" /></div>
              <div>
                <div className="text-xs text-muted-foreground">Filed to your RideAtlas on</div>
                <div className="font-semibold text-sm">{new Date(entry.created_at).toLocaleString("en-US", { month: "short", day: "numeric", year: "numeric", hour: "2-digit", minute: "2-digit" })}</div>
              </div>
            </div>
          </div>

          {/* Bottom nav */}
          <div className="mt-12 flex items-center justify-between">
            <Button variant="outline" asChild data-testid="journal-detail-back-bottom">
              <Link to="/journal"><ArrowLeftIcon size={16} className="mr-1.5" /> All entries</Link>
            </Button>
            <Button asChild className="bg-primary hover:bg-primary/90">
              <Link to="/journal" data-testid="journal-detail-new-entry">
                <SparkleIcon size={14} weight="fill" className="mr-1.5" /> New AI entry
              </Link>
            </Button>
          </div>
        </div>
      </div>

      {/* Lightbox */}
      {lightbox && (
        <div
          onClick={() => setLightbox(null)}
          className="fixed inset-0 z-50 bg-background/95 backdrop-blur-md flex items-center justify-center p-6 cursor-zoom-out"
          data-testid="journal-detail-lightbox"
        >
          <img src={lightbox} alt="" className="max-h-full max-w-full object-contain rounded-md shadow-2xl" />
        </div>
      )}
    </article>
  );
}
