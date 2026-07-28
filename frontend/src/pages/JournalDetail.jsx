import React, { useEffect, useState } from "react";
import { useParams, useNavigate, Link } from "react-router-dom";
import client, { formatApiError } from "../lib/api";
import { Button } from "../components/ui/button";
import {
  ArrowLeftIcon, TrashIcon, SparkleIcon, CalendarBlankIcon, MapPinIcon,
  CloudIcon, UsersThreeIcon, HeartIcon, ClockIcon, PathIcon, CompassIcon, QuotesIcon,
} from "@phosphor-icons/react";
import { toast } from "sonner";

function readingTime(text) {
  const words = (text || "").trim().split(/\s+/).filter(Boolean).length;
  return Math.max(1, Math.round(words / 200));
}

function MetaItem({ Icon, label, value }) {
  if (!value) return null;
  return (
    <div className="flex items-start gap-2.5">
      <Icon size={14} weight="duotone" className="text-primary mt-0.5 shrink-0" />
      <div>
        <div className="text-[10px] uppercase tracking-widest text-muted-foreground font-semibold">{label}</div>
        <div className="text-sm font-medium mt-0.5 leading-snug">{value}</div>
      </div>
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
    setEntry(null); setNotFound(false);
    client.get(`/journal/${id}`)
      .then((r) => { if (mounted) setEntry(r.data); })
      .catch(() => { if (mounted) setNotFound(true); });
    return () => { mounted = false; };
  }, [id]);

  useEffect(() => {
    // Ensure page starts at the top when opening any entry
    window.scrollTo({ top: 0 });
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
      <div className="fixed inset-0 bg-background text-foreground flex items-center justify-center text-center p-8 z-10">
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
    return <div className="fixed inset-0 bg-background flex items-center justify-center text-muted-foreground z-10">Loading ride…</div>;
  }

  const hero = entry.photos?.[0];
  const gallery = (entry.photos || []).slice(1);
  const mins = readingTime(entry.notes);
  const dateFmt = entry.date
    ? new Date(entry.date).toLocaleDateString("en-US", { weekday: "long", month: "long", day: "numeric", year: "numeric" })
    : "";

  // Split into paragraphs
  const raw = (entry.notes || "").trim();
  let paragraphChunks = raw.split(/\n\n+/).map((p) => p.trim()).filter(Boolean);
  if (paragraphChunks.length <= 1) {
    const sentences = raw.split(/(?<=[.!?])\s+(?=[A-Z"])/);
    const perPara = Math.max(2, Math.ceil(sentences.length / 4));
    paragraphChunks = [];
    for (let i = 0; i < sentences.length; i += perPara) {
      paragraphChunks.push(sentences.slice(i, i + perPara).join(" ").trim());
    }
  }
  paragraphChunks = paragraphChunks.filter(Boolean);
  const pullQuote = paragraphChunks.length >= 2
    ? paragraphChunks[Math.min(1, paragraphChunks.length - 1)].split(/(?<=[.!?])\s+/)[0]
    : null;

  return (
    <div className="fixed inset-0 z-30 bg-background text-foreground overflow-y-auto">
      {/* Top bar */}
      <header className="sticky top-0 z-50 backdrop-blur-md bg-background/85 border-b border-border/60">
        <div className="max-w-6xl mx-auto px-6 h-14 flex items-center justify-between">
          <button
            onClick={() => navigate("/journal")}
            data-testid="journal-detail-back"
            className="flex items-center gap-2 text-sm text-muted-foreground hover:text-foreground transition-colors"
          >
            <ArrowLeftIcon size={16} /> <span className="hidden sm:inline">All entries</span>
          </button>
          <Link to="/dashboard" className="flex items-center gap-2" data-testid="journal-detail-brand">
            <CompassIcon size={18} weight="duotone" className="text-primary" />
            <span className="font-display text-sm font-bold tracking-tight">RideAtlas Journal</span>
          </Link>
          <button
            onClick={del}
            data-testid="journal-detail-delete"
            className="flex items-center gap-1.5 text-sm text-muted-foreground hover:text-destructive transition-colors"
          >
            <TrashIcon size={14} /> <span className="hidden sm:inline">Delete</span>
          </button>
        </div>
      </header>

      {/* Hero */}
      {hero ? (
        <section className="relative w-full bg-background">
          <div className="relative w-full h-[55vh] md:h-[72vh] max-h-[820px] overflow-hidden bg-muted">
            <img
              src={hero}
              alt={entry.title}
              className="w-full h-full object-cover"
              data-testid="journal-detail-hero-image"
            />
            <div className="absolute inset-0 bg-gradient-to-t from-background via-background/70 to-background/20" />
            <div className="absolute inset-x-0 bottom-0">
              <div className="max-w-3xl mx-auto px-6 pb-10 md:pb-16">
                <div className="flex flex-wrap items-center gap-2 mb-5">
                  {entry.ai_generated && (
                    <span className="inline-flex items-center gap-1 text-[10px] px-2 py-0.5 rounded-full bg-primary/25 text-primary border border-primary/40 backdrop-blur-sm font-semibold tracking-wide uppercase">
                      <SparkleIcon size={11} weight="fill" /> AI-narrated
                    </span>
                  )}
                  {entry.location && (
                    <span className="inline-flex items-center gap-1 text-xs px-2 py-0.5 rounded-full bg-background/70 text-muted-foreground border border-border backdrop-blur-sm">
                      <MapPinIcon size={12} weight="duotone" /> {entry.location}
                    </span>
                  )}
                </div>
                <h1
                  data-testid="journal-detail-title"
                  className="font-display text-4xl md:text-6xl lg:text-7xl font-extrabold tracking-tighter leading-[1.02] max-w-3xl"
                  style={{ textShadow: "0 2px 20px rgba(0,0,0,0.55)" }}
                >
                  {entry.title}
                </h1>
                <div className="flex flex-wrap items-center gap-x-5 gap-y-2 mt-6 text-sm text-foreground/85">
                  <span className="flex items-center gap-1.5"><CalendarBlankIcon size={14} weight="duotone" /> {dateFmt}</span>
                  <span className="text-muted-foreground">·</span>
                  <span className="flex items-center gap-1.5"><ClockIcon size={14} weight="duotone" /> {mins} min read</span>
                  {entry.distance_km > 0 && (
                    <>
                      <span className="text-muted-foreground">·</span>
                      <span className="flex items-center gap-1.5 number-tabular"><PathIcon size={14} weight="duotone" /> {entry.distance_km} km</span>
                    </>
                  )}
                </div>
              </div>
            </div>
          </div>
        </section>
      ) : (
        <section className="relative bg-background border-b border-border">
          <div className="max-w-3xl mx-auto px-6 pt-16 pb-12">
            <div className="flex flex-wrap items-center gap-2 mb-5">
              {entry.ai_generated && (
                <span className="inline-flex items-center gap-1 text-[10px] px-2 py-0.5 rounded-full bg-primary/10 text-primary border border-primary/30 font-semibold uppercase tracking-wide">
                  <SparkleIcon size={11} weight="fill" /> AI-narrated
                </span>
              )}
              {entry.location && (
                <span className="text-xs text-muted-foreground flex items-center gap-1">
                  <MapPinIcon size={12} weight="duotone" /> {entry.location}
                </span>
              )}
            </div>
            <h1 data-testid="journal-detail-title" className="font-display text-4xl md:text-6xl font-extrabold tracking-tighter leading-[1.02]">
              {entry.title}
            </h1>
            <div className="flex flex-wrap items-center gap-x-5 gap-y-2 mt-5 text-sm text-muted-foreground">
              <span className="flex items-center gap-1.5"><CalendarBlankIcon size={14} weight="duotone" /> {dateFmt}</span>
              <span>·</span>
              <span className="flex items-center gap-1.5"><ClockIcon size={14} weight="duotone" /> {mins} min read</span>
            </div>
          </div>
        </section>
      )}

      {/* Article body */}
      <main className="relative bg-background">
        <div className="max-w-3xl mx-auto px-6 py-12 md:py-20">
          {/* Meta strip - horizontal chips just under hero */}
          <div className="flex flex-wrap gap-x-8 gap-y-4 pb-8 mb-10 border-b border-border">
            <MetaItem Icon={MapPinIcon} label="Where" value={entry.location} />
            <MetaItem Icon={CloudIcon} label="Weather" value={entry.weather} />
            <MetaItem Icon={UsersThreeIcon} label="Company" value={entry.companions} />
            <MetaItem Icon={HeartIcon} label="Mood" value={entry.mood} />
            <MetaItem
              Icon={ClockIcon}
              label="Duration"
              value={entry.duration_min > 0 ? `${Math.floor(entry.duration_min / 60)}h ${entry.duration_min % 60}m` : null}
            />
            <MetaItem
              Icon={PathIcon}
              label="Distance"
              value={entry.distance_km > 0 ? `${entry.distance_km} km` : null}
            />
          </div>

          {/* Prose */}
          <div className="prose-editorial">
            {paragraphChunks.map((para, i) => (
              <React.Fragment key={i}>
                <p
                  className={`font-body text-[1.2rem] md:text-[1.28rem] leading-[1.78] text-foreground/90 mb-7 ${
                    i === 0 ? "first-letter:font-display first-letter:text-[5.5rem] first-letter:font-bold first-letter:text-primary first-letter:float-left first-letter:leading-[0.85] first-letter:pr-3 first-letter:pt-1 first-letter:mr-1" : ""
                  }`}
                  data-testid={`journal-detail-paragraph-${i}`}
                >
                  {para}
                </p>
                {i === 0 && pullQuote && paragraphChunks.length > 2 && (
                  <blockquote className="my-12 py-2 border-l-4 border-primary pl-6 md:pl-8">
                    <QuotesIcon size={22} weight="fill" className="text-primary/50 mb-2" />
                    <p className="font-display text-xl md:text-2xl italic leading-snug text-foreground/95">
                      {pullQuote}
                    </p>
                  </blockquote>
                )}
                {/* Inline photo after 2nd paragraph */}
                {i === 1 && gallery[0] && (
                  <figure className="my-12 -mx-6 md:mx-0">
                    <button
                      onClick={() => setLightbox(gallery[0])}
                      className="block w-full overflow-hidden rounded-none md:rounded-md border-y md:border border-border"
                      data-testid="journal-detail-inline-photo"
                    >
                      <img src={gallery[0]} alt="" className="w-full h-auto object-cover max-h-[520px]" />
                    </button>
                    {entry.location && (
                      <figcaption className="text-xs text-muted-foreground text-center mt-3 italic">
                        {entry.location}
                      </figcaption>
                    )}
                  </figure>
                )}
              </React.Fragment>
            ))}
          </div>

          {/* Remaining gallery */}
          {gallery.length > 1 && (
            <div className="mt-14 pt-10 border-t border-border">
              <div className="overline mb-6">More from this ride</div>
              <div className={`grid gap-3 ${gallery.slice(1).length === 1 ? "grid-cols-1" : "grid-cols-2"}`}>
                {gallery.slice(1).map((src, i) => (
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

          {/* Signature block */}
          <div className="mt-16 pt-8 border-t border-border">
            <div className="flex items-center gap-4">
              <div className="p-3 rounded-md bg-primary/10 text-primary shrink-0">
                <CompassIcon size={22} weight="duotone" />
              </div>
              <div className="min-w-0">
                <div className="text-xs text-muted-foreground">Filed to your atlas on</div>
                <div className="font-semibold text-sm">
                  {new Date(entry.created_at).toLocaleString("en-US", { month: "short", day: "numeric", year: "numeric", hour: "2-digit", minute: "2-digit" })}
                </div>
              </div>
            </div>
          </div>

          {/* Bottom actions */}
          <div className="mt-14 flex flex-col sm:flex-row items-stretch sm:items-center justify-between gap-3">
            <Button variant="outline" asChild data-testid="journal-detail-back-bottom" className="sm:min-w-[180px]">
              <Link to="/journal"><ArrowLeftIcon size={16} className="mr-1.5" /> All entries</Link>
            </Button>
            <Button asChild className="bg-primary hover:bg-primary/90 sm:min-w-[180px]" data-testid="journal-detail-new-entry">
              <Link to="/journal">
                <SparkleIcon size={14} weight="fill" className="mr-1.5" /> New AI entry
              </Link>
            </Button>
          </div>
        </div>
      </main>

      {/* Lightbox */}
      {lightbox && (
        <div
          onClick={() => setLightbox(null)}
          className="fixed inset-0 z-[60] bg-background/95 backdrop-blur-md flex items-center justify-center p-6 cursor-zoom-out"
          data-testid="journal-detail-lightbox"
        >
          <img src={lightbox} alt="" className="max-h-full max-w-full object-contain rounded-md shadow-2xl" />
        </div>
      )}
    </div>
  );
}
