import React from "react";
import { BrowserRouter, Routes, Route, Navigate, Outlet } from "react-router-dom";
import { AuthProvider, useAuth } from "./context/AuthContext";
import { Toaster } from "sonner";

import Landing from "./pages/Landing";
import Login from "./pages/Login";
import Register from "./pages/Register";
import AppShell from "./components/AppShell";
import Dashboard from "./pages/Dashboard";
import Garage from "./pages/Garage";
import TripPlanner from "./pages/TripPlanner";
import FuelLog from "./pages/FuelLog";
import ServiceHistory from "./pages/ServiceHistory";
import Expenses from "./pages/Expenses";
import Journal from "./pages/Journal";
import JournalDetail from "./pages/JournalDetail";
import Statistics from "./pages/Statistics";
import Packing from "./pages/Packing";

function Protected() {
  const { user, loading } = useAuth();
  if (loading) return <div className="min-h-screen bg-background flex items-center justify-center text-muted-foreground">Loading…</div>;
  if (!user) return <Navigate to="/login" replace />;
  return <AppShell><Outlet /></AppShell>;
}

function PublicOnly({ children }) {
  const { user, loading } = useAuth();
  if (loading) return null;
  if (user) return <Navigate to="/dashboard" replace />;
  return children;
}

export default function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Toaster theme="dark" position="top-right" richColors />
        <Routes>
          <Route path="/" element={<Landing />} />
          <Route path="/login" element={<PublicOnly><Login /></PublicOnly>} />
          <Route path="/register" element={<PublicOnly><Register /></PublicOnly>} />
          <Route element={<Protected />}>
            <Route path="/dashboard" element={<Dashboard />} />
            <Route path="/garage" element={<Garage />} />
            <Route path="/trips" element={<TripPlanner />} />
            <Route path="/fuel" element={<FuelLog />} />
            <Route path="/service" element={<ServiceHistory />} />
            <Route path="/expenses" element={<Expenses />} />
            <Route path="/journal" element={<Journal />} />
            <Route path="/journal/:id" element={<JournalDetail />} />
            <Route path="/stats" element={<Statistics />} />
            <Route path="/packing" element={<Packing />} />
          </Route>
          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}
