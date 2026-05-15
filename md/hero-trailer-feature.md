# Hero Trailer Feature Implementation

## Overview
Added play button functionality to all hero banners across the app that plays trailers with smooth transitions and mute/unmute controls.

## Changes Made

### 1. Created Reusable Trailer Component
**File:** `Views/Shared/_HeroTrailer.cshtml`
- Displays poster image by default
- Shows centered play button when trailer is available
- Smooth fade transition from poster to video when play is clicked
- Mute/unmute button appears in bottom-right corner during playback
- Video loops automatically
- Returns to poster when video ends
- Fully responsive design

### 2. Updated Models
**File:** `Models/HomeViewModel.cs`
- Added `HeroTrailerUrl` property to pass trailer URL to home page

### 3. Updated Controllers
**File:** `Controllers/HomeController.cs`
- Added logic to extract trailer URL from hero item
- For Series: uses `TrailerUrl` from GeneralSeries
- For Movies: uses `VideoUrl` from MediaItem
- Passes trailer URL to view via HomeViewModel

### 4. Updated Views

#### Home Page
**File:** `Views/Home/Index.cshtml`
- Integrated `_HeroTrailer` partial view
- Removed old static background image
- Hero now supports trailer playback

#### Movie Details
**File:** `Views/Movie/Details.cshtml`
- Integrated `_HeroTrailer` partial with VideoUrl
- Removed old background loading script
- Hero banner now plays movie video as trailer

#### Series Details
**File:** `Views/Series/GetSeriesById.cshtml`
- Integrated `_HeroTrailer` partial with TrailerUrl
- Removed old background loading script
- Hero banner now plays series trailer

## Features

### Play Button
- 80px circular button with glassmorphism effect
- Centered on hero image
- Smooth scale animation on hover
- Hides when video starts playing
- Reappears when video ends

### Video Playback
- Smooth 0.6s fade transition from poster to video
- Video covers entire hero area (object-fit: cover)
- Muted by default for autoplay compatibility
- Loops automatically
- Returns to poster on completion

### Mute/Unmute Control
- 48px circular button in bottom-right corner
- Only visible during video playback
- Toggle between muted and unmuted states
- Visual icon changes based on state
- Glassmorphism background with backdrop blur

### Responsive Design
- Smaller buttons on mobile (60px play, 40px mute)
- Adjusted positioning for mobile screens
- Touch-friendly interaction areas

## Technical Details

### CSS Classes
- `.hero-trailer-wrap` - Container for entire trailer system
- `.hero-trailer-poster` - Background poster image
- `.hero-trailer-video` - Video element
- `.hero-play-btn` - Centered play button
- `.hero-mute-btn` - Mute/unmute toggle
- `.playing` - Applied to video when active
- `.hidden` - Hides elements smoothly
- `.visible` - Shows elements smoothly

### JavaScript Functionality
- Loads poster image with fade-in effect
- Handles play button click to start video
- Manages video state transitions
- Toggles mute/unmute with icon updates
- Resets to poster when video ends
- Error handling for playback failures

## Browser Compatibility
- Uses modern CSS features (backdrop-filter, object-fit)
- Graceful degradation for older browsers
- Tested with HTML5 video element
- Supports all major video formats

## Performance
- Lazy loading with `preload="none"`
- Smooth CSS transitions
- Minimal JavaScript overhead
- Efficient event handling
