import React from "react";
import { NavLink, useNavigate, Link } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import {
  GaugeIcon, MotorcycleIcon, MapTrifoldIcon, GasPumpIcon, WrenchIcon,
  ReceiptIcon, NotebookIcon, ChartLineUpIcon, BackpackIcon, SignOutIcon, CompassIcon,
} from "@phosphor-icons/react";

const nav = [
  { to: "/dashboard", label: "Dashboard", Icon: GaugeIcon },
  { to: "/garage", label: "Garage", Icon: MotorcycleIcon },
  { to: "/trips", label: "Trip Planner", Icon: MapTrifoldIcon },
  { to: "/fuel", label: "Fuel Log", Icon: GasPumpIcon },
  { to: "/service", label: "Service", Icon: WrenchIcon },
  { to: "/expenses", label: "Expenses", Icon: ReceiptIcon },
  { to: "/journal", label: "Journal", Icon: NotebookIcon },
  { to: "/stats", label: "Statistics", Icon: ChartLineUpIcon },
  { to: "/packing", label: "Packing", Icon: BackpackIcon },
];

export default function AppShell({ children }) {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = async () => {
    await logout();
    navigate("/");
  };

  return (
    <div className="min-h-screen bg-background text-foreground">
      <div className="grid grid-cols-[240px_1fr] min-h-screen">
        {/* Sidebar */}
        <aside className="border-r border-border bg-card/60 backdrop-blur-md flex flex-col">
          <Link to="/dashboard" className="px-6 py-6 border-b border-border flex items-center gap-2" data-testid="sidebar-brand">
            <CompassIcon size={28} weight="duotone" className="text-primary" />
            <div>
              <div className="font-display text-lg font-bold tracking-tight leading-none">RideAtlas</div>
              <div className="overline mt-1">Plan · Ride · Remember</div>
            </div>
          </Link>
          <nav className="flex-1 py-4 px-3 space-y-1">
            {nav.map(({ to, label, Icon }) => (
              <NavLink
                key={to}
                to={to}
                data-testid={`nav-${to.slice(1)}`}
                className={({ isActive }) =>
                  `flex items-center gap-3 px-3 py-2.5 rounded-md text-sm font-medium transition-colors border-l-2 ${
                    isActive
                      ? "border-primary bg-primary/10 text-foreground"
                      : "border-transparent text-muted-foreground hover:text-foreground hover:bg-muted/40"
                  }`
                }
              >
                <Icon size={20} weight="duotone" />
                <span>{label}</span>
              </NavLink>
            ))}
          </nav>
          <div className="px-3 py-4 border-t border-border">
            <div className="px-3 py-2 mb-2">
              <div className="text-xs text-muted-foreground">Signed in as</div>
              <div className="text-sm font-semibold truncate" data-testid="current-user-name">{user?.name}</div>
              <div className="text-xs text-muted-foreground truncate">{user?.email}</div>
            </div>
            <button
              onClick={handleLogout}
              data-testid="logout-button"
              className="w-full flex items-center gap-2 px-3 py-2 rounded-md text-sm text-muted-foreground hover:text-foreground hover:bg-muted/40 transition-colors"
            >
              <SignOutIcon size={18} weight="duotone" />
              Sign out
            </button>
          </div>
        </aside>

        {/* Main content */}
        <main className="min-w-0 bg-topo-lines">
          {children}
        </main>
      </div>
    </div>
  );
}
