// hooks/useSecureStorage.ts
import { useState, useEffect, useCallback } from 'react';
import { secureStorage } from '../utils/secureStorage';

export function useSecureStorage<T>(key: string, initialValue?: T) {
    const [storedValue, setStoredValue] = useState<T | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    // Load value on mount
    useEffect(() => {
        loadValue();
    }, [key]);

    const loadValue = useCallback(async () => {
        try {
            setLoading(true);
            setError(null);
            
            const value = await secureStorage.getItem<T>(key);
            if (value !== null) {
                setStoredValue(value);
            } else if (initialValue !== undefined) {
                setStoredValue(initialValue);
            } else {
                setStoredValue(null);
            }
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Failed to load data');
            setStoredValue(initialValue || null);
        } finally {
            setLoading(false);
        }
    }, [key, initialValue]);

    const setValue = useCallback(async (value: T | ((val: T | null) => T)) => {
        try {
            setError(null);
            
            // Allow value to be a function (like useState)
            const valueToStore = value instanceof Function ? value(storedValue) : value;
            
            // Save to secure storage
            await secureStorage.setItem(key, valueToStore);
            
            // Update state
            setStoredValue(valueToStore);
            
            return true;
        } catch (err) {
            const errorMessage = err instanceof Error ? err.message : 'Failed to save data';
            setError(errorMessage);
            return false;
        }
    }, [key, storedValue]);

    const removeValue = useCallback(async () => {
        try {
            secureStorage.removeItem(key);
            setStoredValue(initialValue || null);
            setError(null);
            return true;
        } catch (err) {
            const errorMessage = err instanceof Error ? err.message : 'Failed to remove data';
            setError(errorMessage);
            return false;
        }
    }, [key, initialValue]);

    const refresh = useCallback(() => {
        return loadValue();
    }, [loadValue]);

    return {
        value: storedValue,
        setValue,
        removeValue,
        loading,
        error,
        refresh,
        hasValue: storedValue !== null,
    };
}