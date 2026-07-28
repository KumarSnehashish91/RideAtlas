import React from "react";

export function PageHeader({ overline, title, description, right }) {
  return (
    <div className="flex flex-col md:flex-row md:items-end md:justify-between gap-4 mb-8">
      <div>
        {overline && <div className="overline mb-2">{overline}</div>}
        <h1 className="font-display text-3xl md:text-4xl font-bold tracking-tight">{title}</h1>
        {description && <p className="text-muted-foreground mt-2 max-w-2xl">{description}</p>}
      </div>
      {right && <div className="flex gap-2">{right}</div>}
    </div>
  );
}

export function StatTile({ label, value, unit, tone = "default", testid }) {
  const toneClass = tone === "primary" ? "text-primary" : tone === "accent" ? "text-accent" : "text-foreground";
  return (
    <div className="p-5 rounded-md border border-border bg-card/60 hover:border-primary/40 transition-colors" data-testid={testid}>
      <div className="overline">{label}</div>
      <div className={`font-display text-3xl font-bold mt-2 number-tabular ${toneClass}`}>
        {value}
        {unit && <span className="text-base text-muted-foreground font-medium ml-1">{unit}</span>}
      </div>
    </div>
  );
}

export function EmptyState({ title, description, action }) {
  return (
    <div className="border border-dashed border-border rounded-md p-12 text-center bg-card/30">
      <h3 className="font-display text-xl font-semibold">{title}</h3>
      {description && <p className="text-muted-foreground mt-2 max-w-md mx-auto">{description}</p>}
      {action && <div className="mt-6 flex justify-center">{action}</div>}
    </div>
  );
}

export function Section({ title, right, children }) {
  return (
    <section className="mb-10">
      {(title || right) && (
        <div className="flex items-center justify-between mb-4">
          {title && <h2 className="font-display text-xl font-semibold">{title}</h2>}
          {right}
        </div>
      )}
      {children}
    </section>
  );
}

export function PageContainer({ children }) {
  return <div className="p-6 md:p-10 max-w-7xl">{children}</div>;
}
