// services/fileService.ts
import { FILES_API_BASE_URL } from './api';
const TOKEN_KEY = 'auth_token';

export interface FileUploadResponse {
	id: string;
	url: string;
}

export const fileService = {
	uploadFile: async (file: File): Promise<FileUploadResponse> => {
		const token = localStorage.getItem(TOKEN_KEY);
		const formData = new FormData();
		formData.append('file', file);

		const response = await fetch(`${FILES_API_BASE_URL}/api/db1/files`, {
			method: 'POST',
			headers: {
				...(token && { Authorization: `Bearer ${token}` }),
			},
			body: formData,
		});

		if (!response.ok) {
			const errorText = await response.text();
			throw new Error(`Upload Error (${response.status}): ${errorText || response.statusText}`);
		}

		const data = await response.json();

		// The final URL to sent with Post is https://robosa.co.za/api/db1/files/37e3b78c-55c6-4532-a4aa-469551ab278b
		// The response example says url is "/api/db1/files/37e3b78c-55c6-4532-a4aa-469551ab278b"
		// We should return the full URL if it doesn't already have the base.

		const fullUrl = data.url.startsWith('http')
			? data.url
			: `${FILES_API_BASE_URL}${data.url.startsWith('/') ? '' : '/'}${data.url}`;

		return {
			...data,
			url: fullUrl
		};
	},
};