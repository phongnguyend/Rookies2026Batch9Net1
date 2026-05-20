import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';
import { ENV_CONFIGS } from '../config/env';

export const baseApiSlice = createApi({
    reducerPath: 'api',
    baseQuery: fetchBaseQuery({
        baseUrl: ENV_CONFIGS.apiUrl,
        credentials: 'include', // send cookie automatically
    }),
    tagTypes: ['Account', 'Asset', 'Assignment', 'Report', 'Return'],
    endpoints: () => ({
    }),
});
