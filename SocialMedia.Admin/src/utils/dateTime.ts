/**
 * Formats a date to show both absolute and relative time
 * Example: "12 Jan 2025 at 20:32, 3min ago"
 */
export function formatDateTime(dateString: string): string {
	const date = new Date(dateString);
	const now = new Date();

	// Calculate time difference in milliseconds
	const diffMs = now.getTime() - date.getTime();
	const diffSec = Math.floor(diffMs / 1000);
	const diffMin = Math.floor(diffSec / 60);
	const diffHour = Math.floor(diffMin / 60);
	const diffDay = Math.floor(diffHour / 24);
	const diffMonth = Math.floor(diffDay / 30);
	const diffYear = Math.floor(diffDay / 365);

	// Format relative time
	let relativeTime: string;
	if (diffSec < 60) {
		relativeTime = 'just now';
	} else if (diffMin < 60) {
		relativeTime = `${diffMin}min ago`;
	} else if (diffHour < 24) {
		relativeTime = `${diffHour}h ago`;
	} else if (diffDay < 30) {
		relativeTime = `${diffDay}d ago`;
	} else if (diffMonth < 12) {
		relativeTime = `${diffMonth}mon ago`;
	} else {
		relativeTime = `${diffYear}y ago`;
	}

	// Format absolute time
	const months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
	const day = date.getDate();
	const month = months[date.getMonth()];
	const year = date.getFullYear();
	const hours = date.getHours().toString().padStart(2, '0');
	const minutes = date.getMinutes().toString().padStart(2, '0');

	const absoluteTime = `${day} ${month} ${year} at ${hours}:${minutes}`;

	return `${absoluteTime}, ${relativeTime}`;
}

export const formatDate = (dateString?: string) => {
	if (!dateString) return 'Never';
	const date = new Date(dateString);
	return date.toLocaleDateString('en-US', {
		month: 'short',
		day: 'numeric',
		year: 'numeric',
		hour: '2-digit',
		minute: '2-digit',
	});
};