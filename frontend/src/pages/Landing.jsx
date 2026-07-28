import React from "react";
import { Link } from "react-router-dom";
import { Button } from "../components/ui/button";
import {
  CompassIcon, MotorcycleIcon, MapTrifoldIcon, GasPumpIcon, WrenchIcon,
  NotebookIcon, ChartLineUpIcon, ShieldCheckIcon, LightningIcon, MountainsIcon,
} from "@phosphor-icons/react";

const FEATURES = [
  { Icon: MapTrifoldIcon, title: "Trip Planner", body: "Multi-day itineraries with fuel stops, overnights, and interactive routes on an offline-first map." },
  { Icon: MotorcycleIcon, title: "Garage", body: "Track multiple bikes with odometer, insurance, PUC, mileage and predictive service intervals." },
  { Icon: GasPumpIcon, title: "Smart Fuel Log", body: "Fill-ups auto-compute mileage and cost per km. Spot mileage trends across months." },
  { Icon: WrenchIcon, title: "Service History", body: "Every oil change, chain lube, tyre swap — logged, searchable, and reminded." },
  { Icon: NotebookIcon, title: "Ride Journal", body: "Preserve rides with mood, weather, companions and distance. Your atlas of adventures." },
  { Icon: ChartLineUpIcon, title: "Statistics", body: "Total km, longest ride, riding days, spend breakdown — the full story of your riding life." },
];

const PERSONAS = [
  { title: "Adventure riders", desc: "Non-paved, high altitude, zero-cellular corridors — reliability is everything." },
  { title: "Long-distance tourers", desc: "Multi-state highways, split budgets, dense fuel analytics." },
  { title: "Weekend riders", desc: "Curated routes and quick weekend loops with clean summaries." },
  { title: "Club leads", desc: "Standardised manifests, shared packing lists and route exports." },
];

export default function Landing() {
  return (
    <div className="min-h-screen bg-background text-foreground">
      {/* Header */}
      <header className="sticky top-0 z-40 border-b border-border/60 backdrop-blur-md bg-background/70">
        <div className="max-w-7xl mx-auto px-6 h-16 flex items-center justify-between">
          <div className="flex items-center gap-2">
            <CompassIcon size={26} weight="duotone" className="text-primary" />
            <span className="font-display text-lg font-bold tracking-tight">RideAtlas</span>
          </div>
          <nav className="hidden md:flex items-center gap-8 text-sm text-muted-foreground">
            <a href="#features" className="hover:text-foreground transition-colors">Features</a>
            <a href="#personas" className="hover:text-foreground transition-colors">Who it's for</a>
            <a href="#lifecycle" className="hover:text-foreground transition-colors">Lifecycle</a>
          </nav>
          <div className="flex items-center gap-3">
            <Link to="/login" data-testid="header-login-link" className="text-sm text-muted-foreground hover:text-foreground transition-colors">Sign in</Link>
            <Button asChild data-testid="header-get-started-btn" className="bg-primary hover:bg-primary/90">
              <Link to="/register">Get started</Link>
            </Button>
          </div>
        </div>
      </header>

      {/* Hero */}
      <section className="relative overflow-hidden bg-topo">
        <div className="max-w-7xl mx-auto px-6 py-20 md:py-28 grid md:grid-cols-12 gap-10 items-center">
          <div className="md:col-span-7">
            <div className="overline text-primary mb-4">v1.0 · Premium Touring Ecosystem</div>
            <h1 className="font-display text-5xl md:text-6xl lg:text-7xl font-extrabold tracking-tighter leading-[0.95]">
              Plan. Ride.<br />
              <span className="text-primary">Record.</span> Remember.
            </h1>
            <p className="mt-6 text-lg text-muted-foreground max-w-xl leading-relaxed">
              The definitive digital atlas for motorcycle touring. One home for every trip, every service, every liter, every kilometer — and every story worth telling.
            </p>
            <div className="mt-8 flex flex-col sm:flex-row gap-3">
              <Button asChild size="lg" data-testid="hero-get-started-btn" className="bg-primary hover:bg-primary/90 h-12 px-6">
                <Link to="/register">Start your atlas</Link>
              </Button>
              <Button asChild size="lg" variant="outline" data-testid="hero-signin-btn" className="h-12 px-6">
                <Link to="/login">I already have an account</Link>
              </Button>
            </div>
            <div className="mt-10 grid grid-cols-3 gap-6 max-w-md">
              <div>
                <div className="font-display text-2xl font-bold text-primary">17</div>
                <div className="text-xs text-muted-foreground mt-1">integrated modules</div>
              </div>
              <div>
                <div className="font-display text-2xl font-bold text-primary">5</div>
                <div className="text-xs text-muted-foreground mt-1">lifecycle pillars</div>
              </div>
              <div>
                <div className="font-display text-2xl font-bold text-primary">∞</div>
                <div className="text-xs text-muted-foreground mt-1">roads to remember</div>
              </div>
            </div>
          </div>
          <div className="md:col-span-5">
            <div className="relative aspect-[4/5] rounded-lg overflow-hidden border border-border bg-card">
              <img
                src="https://images.unsplash.com/photo-1574417462817-a4e1aa71cc78?w=800&auto=format&fit=crop&q=80"
                alt="Motorcyclists on tour"
                className="w-full h-full object-cover"
              />
              <div className="absolute inset-0 bg-gradient-to-t from-background via-background/30 to-transparent" />
              <div className="absolute bottom-6 left-6 right-6">
                <div className="overline text-primary mb-1">Next ride · 6 days</div>
                <div className="font-display text-2xl font-bold">Delhi → Manali</div>
                <div className="text-sm text-muted-foreground">560 km · 2 riders · ₹12,000 budget</div>
              </div>
            </div>
          </div>
        </div>
      </section>

      {/* Features */}
      <section id="features" className="py-24 border-t border-border">
        <div className="max-w-7xl mx-auto px-6">
          <div className="max-w-2xl mb-16">
            <div className="overline mb-3">One app · Every ride</div>
            <h2 className="font-display text-3xl md:text-5xl font-bold tracking-tight">Built for the way riders actually ride.</h2>
            <p className="text-muted-foreground mt-4">Nine focused modules that talk to each other, so you spend less time in menus and more time on the road.</p>
          </div>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            {FEATURES.map(({ Icon, title, body }) => (
              <div key={title} className="p-6 border border-border rounded-md bg-card/60 hover:border-primary/40 hover:-translate-y-0.5 transition-transform">
                <Icon size={28} weight="duotone" className="text-primary" />
                <h3 className="font-display text-lg font-semibold mt-4">{title}</h3>
                <p className="text-sm text-muted-foreground mt-2 leading-relaxed">{body}</p>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* Lifecycle */}
      <section id="lifecycle" className="py-24 bg-card/40 border-t border-border">
        <div className="max-w-7xl mx-auto px-6">
          <div className="max-w-2xl mb-14">
            <div className="overline mb-3">The 5-pillar lifecycle</div>
            <h2 className="font-display text-3xl md:text-5xl font-bold tracking-tight">Plan → Manage → Record → Remember → Improve</h2>
          </div>
          <div className="grid md:grid-cols-5 gap-4">
            {[
              { n: "01", t: "Plan", d: "Itineraries, fuel stops, budget & packing." },
              { n: "02", t: "Manage", d: "Bikes, docs, service intervals & mileage." },
              { n: "03", t: "Record", d: "Rides, fuel fill-ups, expenses on the go." },
              { n: "04", t: "Remember", d: "Journal, photos, weather & mood." },
              { n: "05", t: "Improve", d: "Statistics, achievements, and TCO." },
            ].map((s) => (
              <div key={s.n} className="p-6 border border-border rounded-md bg-background">
                <div className="text-primary font-display text-2xl font-bold">{s.n}</div>
                <div className="font-display text-lg font-semibold mt-2">{s.t}</div>
                <div className="text-sm text-muted-foreground mt-2">{s.d}</div>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* Personas */}
      <section id="personas" className="py-24 border-t border-border">
        <div className="max-w-7xl mx-auto px-6 grid md:grid-cols-2 gap-14 items-start">
          <div>
            <div className="overline mb-3">Who it's for</div>
            <h2 className="font-display text-3xl md:text-5xl font-bold tracking-tight">If you count roads, not likes.</h2>
            <p className="text-muted-foreground mt-4 max-w-md">RideAtlas is engineered for riders whose stories don't fit on a social feed — and whose weekends deserve better than a spreadsheet.</p>
            <div className="mt-6 flex flex-wrap gap-3 text-xs">
              <span className="inline-flex items-center gap-1.5 px-3 py-1.5 rounded-full border border-border"><ShieldCheckIcon size={14} weight="duotone" className="text-success"/> Encrypted vault</span>
              <span className="inline-flex items-center gap-1.5 px-3 py-1.5 rounded-full border border-border"><LightningIcon size={14} weight="duotone" className="text-accent"/> Fast</span>
              <span className="inline-flex items-center gap-1.5 px-3 py-1.5 rounded-full border border-border"><MountainsIcon size={14} weight="duotone" className="text-primary"/> Offline-friendly</span>
            </div>
          </div>
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            {PERSONAS.map((p) => (
              <div key={p.title} className="p-5 border border-border rounded-md bg-card/60">
                <div className="font-display text-lg font-semibold">{p.title}</div>
                <div className="text-sm text-muted-foreground mt-1.5">{p.desc}</div>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* CTA */}
      <section className="py-20 border-t border-border bg-topo">
        <div className="max-w-4xl mx-auto px-6 text-center">
          <h2 className="font-display text-4xl md:text-5xl font-bold tracking-tight">Your atlas is waiting.</h2>
          <p className="text-muted-foreground mt-3 max-w-xl mx-auto">Free during v1.0 preview. No credit card. Just create your account and drop a pin on your next ride.</p>
          <div className="mt-8 flex flex-col sm:flex-row gap-3 justify-center">
            <Button asChild size="lg" data-testid="cta-get-started-btn" className="bg-primary hover:bg-primary/90 h-12 px-8">
              <Link to="/register">Create free account</Link>
            </Button>
          </div>
        </div>
      </section>

      <footer className="border-t border-border py-8">
        <div className="max-w-7xl mx-auto px-6 flex flex-col md:flex-row items-center justify-between gap-3 text-sm text-muted-foreground">
          <div className="flex items-center gap-2">
            <CompassIcon size={18} weight="duotone" className="text-primary" />
            <span>© 2026 RideAtlas — Plan · Ride · Remember</span>
          </div>
          <div>Built for riders. Not for feeds.</div>
        </div>
      </footer>
    </div>
  );
}
