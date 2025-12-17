// utils/secureStorage.ts
interface SecureStorageOptions {
    keyName?: string;
    ttl?: number; // Time to live in milliseconds
}

class SecureStorage {
    private keyName: string;
    private ttl?: number;
    private keyPromise: Promise<CryptoKey> | null = null;

    constructor(options: SecureStorageOptions = {}) {
        this.keyName = options.keyName || 'secure-storage-key';
        this.ttl = options.ttl;
    }

    private async getKey(): Promise<CryptoKey> {
        if (!this.keyPromise) {
            this.keyPromise = this.generateOrRetrieveKey();
        }
        return this.keyPromise;
    }

    private async generateOrRetrieveKey(): Promise<CryptoKey> {
        try {
            // Try to retrieve existing key
            const storedKey = localStorage.getItem(this.keyName);
            
            if (storedKey) {
                // Convert from base64 to ArrayBuffer
                const keyData = Uint8Array.from(atob(storedKey), c => c.charCodeAt(0));
                
                return await window.crypto.subtle.importKey(
                    'raw',
                    keyData,
                    {
                        name: 'AES-GCM',
                        length: 256,
                    },
                    false, // Not extractable (more secure)
                    ['encrypt', 'decrypt']
                );
            } else {
                // Generate new key
                const key = await window.crypto.subtle.generateKey(
                    {
                        name: 'AES-GCM',
                        length: 256,
                    },
                    true, // Extractable so we can store it
                    ['encrypt', 'decrypt']
                );

                // Export and store the key
                const exported = await window.crypto.subtle.exportKey('raw', key);
                const keyStr = btoa(String.fromCharCode(...new Uint8Array(exported)));
                localStorage.setItem(this.keyName, keyStr);

                return key;
            }
        } catch (error) {
            console.error('Failed to get encryption key:', error);
            throw new Error('Failed to initialize secure storage');
        }
    }

    private async encrypt(data: string): Promise<string> {
        const key = await this.getKey();
        const encoder = new TextEncoder();
        const dataBytes = encoder.encode(data);

        // Generate random IV for each encryption
        const iv = window.crypto.getRandomValues(new Uint8Array(12));

        const encrypted = await window.crypto.subtle.encrypt(
            {
                name: 'AES-GCM',
                iv: iv,
            },
            key,
            dataBytes
        );

        // Combine IV and encrypted data
        const combined = new Uint8Array(iv.length + encrypted.byteLength);
        combined.set(iv);
        combined.set(new Uint8Array(encrypted), iv.length);

        return btoa(String.fromCharCode(...combined));
    }

    private async decrypt(encryptedData: string): Promise<string> {
        const key = await this.getKey();
        
        // Convert from base64
        const combined = Uint8Array.from(atob(encryptedData), c => c.charCodeAt(0));
        
        // Extract IV (first 12 bytes) and encrypted data
        const iv = combined.slice(0, 12);
        const data = combined.slice(12);

        const decrypted = await window.crypto.subtle.decrypt(
            {
                name: 'AES-GCM',
                iv: iv,
            },
            key,
            data
        );

        const decoder = new TextDecoder();
        return decoder.decode(decrypted);
    }

    async setItem<T>(key: string, value: T): Promise<void> {
        try {
            const data = {
                value,
                timestamp: Date.now(),
                ttl: this.ttl,
            };

            const encrypted = await this.encrypt(JSON.stringify(data));
            localStorage.setItem(key, encrypted);
        } catch (error) {
            console.error('Failed to set secure item:', error);
            throw new Error('Failed to store data securely');
        }
    }

    async getItem<T>(key: string): Promise<T | null> {
        try {
            const encrypted = localStorage.getItem(key);
            if (!encrypted) return null;

            const decrypted = await this.decrypt(encrypted);
            const data = JSON.parse(decrypted);

            // Check if data has expired
            if (data.ttl && Date.now() - data.timestamp > data.ttl) {
                this.removeItem(key);
                return null;
            }

            return data.value;
        } catch (error) {
            console.error('Failed to get secure item:', error);
            this.removeItem(key); // Remove corrupted data
            return null;
        }
    }

    removeItem(key: string): void {
        localStorage.removeItem(key);
    }

    clear(): void {
        localStorage.clear();
        // Don't clear the encryption key so we don't lose all data
    }

    clearAll(): void {
        localStorage.clear();
        // Also clear the encryption key
        localStorage.removeItem(this.keyName);
        this.keyPromise = null;
    }

    async hasItem(key: string): Promise<boolean> {
        const item = await this.getItem(key);
        return item !== null;
    }

    async getKeys(): Promise<string[]> {
        return Object.keys(localStorage).filter(key => key !== this.keyName);
    }
}

// Create a singleton instance
export const secureStorage = new SecureStorage({
    keyName: 'app-secure-key',
    ttl: 7 * 24 * 60 * 60 * 1000, // 7 days default TTL
});