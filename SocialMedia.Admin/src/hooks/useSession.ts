// hooks/useSession.ts
import { useState, useEffect, useCallback } from 'react';
import { secureStorage } from '../utils/secureStorage';

interface SessionData {
    sessionId: string;
    userId: string;
    createdAt: number;
    lastActivity: number;
    ipAddress?: string;
    userAgent?: string;
}

export function useSession() {
    const [session, setSession] = useState<SessionData | null>(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        loadSession();
    }, []);

    const loadSession = useCallback(async () => {
        try {
            setLoading(true);
            const sessionData = await secureStorage.getItem<SessionData>('user_session');
            setSession(sessionData);
        } catch (error) {
            console.error('Failed to load session:', error);
        } finally {
            setLoading(false);
        }
    }, []);

    const createSession = useCallback(async (userId: string, metadata?: {
        ipAddress?: string;
        userAgent?: string;
    }) => {
        try {
            const sessionData: SessionData = {
                sessionId: crypto.randomUUID(),
                userId,
                createdAt: Date.now(),
                lastActivity: Date.now(),
                ipAddress: metadata?.ipAddress,
                userAgent: metadata?.userAgent,
            };

            await secureStorage.setItem('user_session', sessionData);
            setSession(sessionData);
            return sessionData.sessionId;
        } catch (error) {
            console.error('Failed to create session:', error);
            return null;
        }
    }, []);

    const updateSessionActivity = useCallback(async () => {
        if (!session) return;
        
        try {
            const updatedSession = {
                ...session,
                lastActivity: Date.now(),
            };
            
            await secureStorage.setItem('user_session', updatedSession);
            setSession(updatedSession);
        } catch (error) {
            console.error('Failed to update session activity:', error);
        }
    }, [session]);

    const clearSession = useCallback(async () => {
        try {
            secureStorage.removeItem('user_session');
            setSession(null);
        } catch (error) {
            console.error('Failed to clear session:', error);
        }
    }, []);

    const isSessionValid = useCallback(() => {
        if (!session) return false;
        
        const SESSION_TIMEOUT = 30 * 60 * 1000; // 30 minutes
        const now = Date.now();
        
        // Check if session has expired due to inactivity
        if (now - session.lastActivity > SESSION_TIMEOUT) {
            clearSession();
            return false;
        }
        
        return true;
    }, [session, clearSession]);

    // Auto-update activity on user interaction
    useEffect(() => {
        const updateOnActivity = () => {
            if (session) {
                updateSessionActivity();
            }
        };

        // Listen for user activity
        window.addEventListener('mousemove', updateOnActivity);
        window.addEventListener('keydown', updateOnActivity);
        window.addEventListener('click', updateOnActivity);
        window.addEventListener('scroll', updateOnActivity);

        return () => {
            window.removeEventListener('mousemove', updateOnActivity);
            window.removeEventListener('keydown', updateOnActivity);
            window.removeEventListener('click', updateOnActivity);
            window.removeEventListener('scroll', updateOnActivity);
        };
    }, [session, updateSessionActivity]);

    // Auto-check session validity periodically
    useEffect(() => {
        if (!session) return;

        const interval = setInterval(() => {
            if (!isSessionValid()) {
                clearSession();
            }
        }, 60000); // Check every minute

        return () => clearInterval(interval);
    }, [session, isSessionValid, clearSession]);

    return {
        session,
        loading,
        createSession,
        clearSession,
        updateSessionActivity,
        isSessionValid: isSessionValid(),
        sessionId: session?.sessionId,
        userId: session?.userId,
    };
}