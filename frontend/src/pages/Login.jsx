import React, { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { formatApiError } from "../lib/api";
import { Button } from "../components/ui/button";
import { Input } from "../components/ui/input";
import { Label } from "../components/ui/label";
import { CompassIcon } from "@phosphor-icons/react";
import { toast } from "sonner";

export default function Login() {
  const { login } = useAuth();
  const navigate = useNavigate();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [busy, setBusy] = useState(false);
  const [err, setErr] = useState("");

  const submit = async (e) => {
    e.preventDefault();
    setBusy(true); setErr("");
    try {
      await login(email, password);
      toast.success("Welcome back to RideAtlas");
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
          src="https://images.unsplash.com/photo-1690540293130-2ca04fbf815b?w=1200&auto=format&fit=crop&q=80"
          alt="Motorcycle touring"
          className="absolute inset-0 w-full h-full object-cover opacity-60"
        />
        <div className="absolute inset-0 bg-gradient-to-br from-background/60 via-background/30 to-background/90" />
        <div className="relative h-full flex flex-col justify-between p-12">
          <Link to="/" className="flex items-center gap-2" data-testid="login-brand-link">
            <CompassIcon size={28} weight="duotone" className="text-primary" />
            <span className="font-display text-xl font-bold">RideAtlas</span>
          </Link>
          <div>
            <div className="overline text-primary mb-3">Welcome back</div>
            <h2 className="font-display text-4xl font-bold tracking-tight leading-tight max-w-md">
              Your next ride is 6 days away. Let's get you set up.
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
          <div className="overline mb-2">Sign in</div>
          <h1 className="font-display text-3xl font-bold tracking-tight">Welcome back, rider.</h1>
          <p className="text-muted-foreground text-sm mt-2">
            Don't have an account?{" "}
            <Link to="/register" data-testid="login-register-link" className="text-primary hover:underline">
              Create one
            </Link>
          </p>

          <form onSubmit={submit} className="mt-8 space-y-4">
            <div className="space-y-2">
              <Label htmlFor="email">Email</Label>
              <Input
                id="email"
                type="email"
                required
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                data-testid="login-email-input"
                placeholder="you@road.com"
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="password">Password</Label>
              <Input
                id="password"
                type="password"
                required
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                data-testid="login-password-input"
                placeholder="••••••••"
              />
            </div>
            {err && <div className="text-sm text-destructive" data-testid="login-error">{err}</div>}
            <Button
              type="submit"
              disabled={busy}
              data-testid="login-submit-button"
              className="w-full bg-primary hover:bg-primary/90 h-11"
            >
              {busy ? "Signing in…" : "Sign in"}
            </Button>
          </form>
        </div>
      </div>
    </div>
  );
}
