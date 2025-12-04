const API_BASE_URL = import.meta.env.VITE_API_URL || 'https://localhost:7168';
const TOKEN_KEY = 'auth_token';

export async function fetchJson<T>(url: string, options?: RequestInit): Promise<T> {
    const token = localStorage.getItem(TOKEN_KEY);

    const response = await fetch(`${API_BASE_URL}${url}`, {
        ...options,
        headers: {
            'Content-Type': 'application/json',
            ...(token && { Authorization: `Bearer ${token}` }),
            ...options?.headers,
        },
    });

    if (!response.ok) {
        throw new Error(`API Error: ${response.statusText}`);
    }

    if (response.status === 204) {
        return {} as T;
    }

    return response.json();
}
