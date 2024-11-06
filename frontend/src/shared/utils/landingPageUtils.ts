import { isAfter, startOfDay } from 'date-fns';
import { toZonedTime } from 'date-fns-tz';

const LANDING_PAGE_PREFERENCE_KEY = 'landingPagePreference';
const LANDING_PAGE_VIEWED_KEY = 'landingPageViewed';

export const setLandingPagePreference = (show: boolean) => {
  const now = new Date().toISOString();
  localStorage.setItem(LANDING_PAGE_PREFERENCE_KEY, JSON.stringify({ show, timestamp: now }));
};

export const setLandingPageViewed = () => {
  sessionStorage.setItem(LANDING_PAGE_VIEWED_KEY, 'true');
};

export const shouldShowLanding = (): boolean => {
  if (sessionStorage.getItem(LANDING_PAGE_VIEWED_KEY) === 'true') {
    return false;
  }

  const preference = localStorage.getItem(LANDING_PAGE_PREFERENCE_KEY);
  if (!preference) return true;

  const { show, timestamp } = JSON.parse(preference);
  if (show) return true;

  const seoulTime = toZonedTime(new Date(), 'Asia/Seoul');
  const savedDate = toZonedTime(new Date(timestamp), 'Asia/Seoul');
  const startOfToday = startOfDay(seoulTime);

  return isAfter(seoulTime, startOfToday) && isAfter(startOfToday, savedDate);
};