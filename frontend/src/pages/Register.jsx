import React, { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { formatApiError } from "../lib/api";
import { Button } from "../components/ui/button";
import { Input } from "../components/ui/input";
import { Label } from "../components/ui/label";
import { CompassIcon } from "@phosphor-icons/react";
import { toast } from "sonner";

export default function Register() {
  const { register } = useAuth();
  const navigate = useNavigate();
  const [name, setName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [busy, setBusy] = useState(false);
  const [err, setErr] = useState("");

  const submit = async (e) => {
    e.preventDefault();
    setBusy(true); setErr("");
    try {
      await register(name, email, password);
      toast.success("Welcome to RideAtlas");
      navigate("/dashboard");
    } catch (e2) {
      const m = formatApiError(e2);
      setErr(m);
      toast.error(m);
    } finally { setBusy(false); }
  };

  return (
    <div className="min-h-screen grid lg:grid-cols-2 bg-background">
      <div className="hidden lg:block relative bg-topo">
        <img
          src="https://images.unsplash.com/photo-1534755563369-ad37931ac77b?w=1200&auto=format&fit=crop&q=80"
          alt="Motorcycle engine"
          className="absolute inset-0 w-full h-full object-cover opacity-50"
        />
        <div className="absolute inset-0 bg-gradient-to-br from-background/70 via-background/40 to-background/95" />
        <div className="relative h-full flex flex-col justify-between p-12">
          <Link to="/" className="flex items-center gap-2" data-testid="register-brand-link">
            <CompassIcon size={28} weight="duotone" className="text-primary" />
            <span className="font-display text-xl font-bold">RideAtlas</span>
          </Link>
          <div>
            <div className="overline text-primary mb-3">Get started · Free during preview</div>
            <h2 className="font-display text-4xl font-bold tracking-tight leading-tight max-w-md">
              Start your atlas of roads, rides and stories worth remembering.
            </h2>
          </div>
        </div>
      </div>

      <div className="flex items-center justify-center p-8">
        <div className="w-full max-w-sm">
          <div className="lg:hidden flex items-center gap-2 mb-8">
            <CompassIcon size={26} weight="duotone" className="text-primary" />
            <span className="font-display text-xl font-bold">RideAtlas</span>
          </div>
          <div className="overline mb-2">Create account</div>
          <h1 className="font-display text-3xl font-bold tracking-tight">Ride with intent.</h1>
          <p className="text-muted-foreground text-sm mt-2">
            Already have an account?{" "}
            <Link to="/login" data-testid="register-login-link" className="text-primary hover:underline">
              Sign in
            </Link>
          </p>

          <form onSubmit={submit} className="mt-8 space-y-4">
            <div className="space-y-2">
              <Label htmlFor="name">Your name</Label>
              <Input id="name" required value={name} onChange={(e) => setName(e.target.value)} data-testid="register-name-input" placeholder="Kumar" />
            </div>
            <div className="space-y-2">
              <Label htmlFor="email">Email</Label>
              <Input id="email" type="email" required value={email} onChange={(e) => setEmail(e.target.value)} data-testid="register-email-input" placeholder="you@road.com" />
            </div>
            <div className="space-y-2">
              <Label htmlFor="password">Password</Label>
              <Input id="password" type="password" required minLength={6} value={password} onChange={(e) => setPassword(e.target.value)} data-testid="register-password-input" placeholder="At least 6 characters" />
            </div>
            {err && <div className="text-sm text-destructive" data-testid="register-error">{err}</div>}
            <Button type="submit" disabled={busy} data-testid="register-submit-button" className="w-full bg-primary hover:bg-primary/90 h-11">
              {busy ? "Creating…" : "Create account"}
            </Button>
          </form>
        </div>
      </div>
    </div>
  );
}
