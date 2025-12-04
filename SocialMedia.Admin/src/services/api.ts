const API_BASE_URL = 'http://localhost:5200/api/v1';

export async function fetchJson<T>(url: string, options?: RequestInit): Promise<T> {
    const response = await fetch(`${API_BASE_URL}${url}`, {
        ...options,
        headers: {
            'Content-Type': 'application/json',
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
