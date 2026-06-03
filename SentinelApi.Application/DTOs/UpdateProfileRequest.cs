namespace SentinelApi.Application.DTOs;

public record UpdateProfileRequest(
    decimal? Latitude,
    decimal? Longitude,
    int RaioKm
);