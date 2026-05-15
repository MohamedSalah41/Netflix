# YouTube Trailer Support Implementation

## Overview
Updated the hero trailer feature to support YouTube, Vimeo, and direct video URLs for movie and series trailers.

## Changes Made

### 1. Updated `_HeroTrailer` Component
**File:** `Views/Shared/_HeroTrailer.cshtml`

#### Added URL Detection Logic
- Detects YouTube URLs (youtube.com, youtu.be)
- Detects Vimeo URLs (vimeo.com)
- Falls back to direct video for other URLs

#### YouTube Embed URL Generation
- Extracts video ID from various YouTube URL formats:
  - `youtube.com/watch?v=VIDEO_ID`
  - `youtu.be/VIDEO_ID`
  - `youtube.com/embed/VIDEO_ID`
- Generates embed URL with parameters:
  - `autoplay=1` - Auto-plays when clicked
  - `mute=1` - Starts muted
  - `controls=0` - Hides YouTube controls for cleaner look
  - `loop=1&playlist=VIDEO_ID` - Loops the video
  - `modestbranding=1` - Minimal YouTube branding
  - `rel=0` - No related videos
  - `showinfo=0` - No video info overlay

#### Vimeo Embed URL Generation
- Extracts video ID from Vimeo URLs
- Generates embed URL with parameters:
  - `autoplay=1` - Auto-plays when clicked
  - `muted=1` - Starts muted
  - `loop=1` - Loops the video
  - `background=1` - Background mode (no controls)

#### Updated HTML Structure
- Conditional rendering based on video type:
  - `<video>` element for direct MP4 files
  - `<iframe>` element for YouTube/Vimeo embeds
- Mute button only shows for direct videos (YouTube/Vimeo handle their own audio)

#### Updated JavaScript
- Detects video type (YouTube, Vimeo, or direct)
- For YouTube/Vimeo: Sets iframe src on play button click
- For direct videos: Uses HTML5 video API
- Smooth fade transitions for all types

### 2. Added TrailerUrl to Movie Model
**File:** `Models/MediaItem.cs`
- Added `TrailerUrl` property to MediaItem class
- Movies now have separate fields:
  - `VideoUrl` - Full movie/episode content
  - `TrailerUrl` - Short trailer for hero banner

### 3. Database Migration
**Migration:** `20260515121000_AddTrailerUrlToMediaItem`
- Added `TrailerUrl` column to Movies table
- Added `TrailerUrl` column to Episodes table
- Default value: empty string

### 4. Updated Movie Forms
**Files:** 
- `Views/Movie/AddMovie.cshtml`
- `Views/Movie/Edit.cshtml`

Added TrailerUrl input field:
```html
<div class="form-field">
    <label asp-for="TrailerUrl" class="form-label"></label>
    <input asp-for="TrailerUrl" class="form-input" 
           placeholder="https://youtu.be/pvKkBUM3Dyc" />
    <span asp-validation-for="TrailerUrl" class="form-field-error"></span>
</div>
```

### 5. Updated Movie Details View
**File:** `Views/Movie/Details.cshtml`
- Changed hero trailer to use `Model.TrailerUrl` instead of `Model.VideoUrl`
- Now displays trailer in hero banner, full movie in player section below

### 6. Updated Home Controller
**File:** `Controllers/HomeController.cs`
- Changed to use `TrailerUrl` for movies instead of `VideoUrl`
- Consistent behavior for both movies and series

## Usage

### For Movies
1. Go to Edit Movie page
2. Enter YouTube URL in TrailerUrl field:
   - `https://youtu.be/pvKkBUM3Dyc`
   - `https://www.youtube.com/watch?v=pvKkBUM3Dyc`
3. Save the movie
4. Visit movie details page
5. Click play button in hero banner to watch trailer

### For Series
- Series already had TrailerUrl field
- Same YouTube URL support applies

## Supported URL Formats

### YouTube
- `https://www.youtube.com/watch?v=VIDEO_ID`
- `https://youtu.be/VIDEO_ID`
- `https://www.youtube.com/embed/VIDEO_ID`

### Vimeo
- `https://vimeo.com/VIDEO_ID`
- `https://player.vimeo.com/video/VIDEO_ID`

### Direct Video
- Any direct MP4 URL
- `https://example.com/video.mp4`

## Example
Movie ID 22 with trailer URL: `https://youtu.be/pvKkBUM3Dyc`
- Visit: `http://localhost:5155/Movie/Details/22`
- Click play button in hero banner
- YouTube trailer plays in embedded iframe
- Smooth fade from poster to video
- Video loops automatically

## Technical Notes

### Why Separate TrailerUrl and VideoUrl?
- **TrailerUrl**: Short promotional video for hero banner (1-3 minutes)
- **VideoUrl**: Full movie content in player section (90+ minutes)
- Allows different sources (YouTube trailer, direct video for full movie)
- Better user experience with quick-loading trailers

### Iframe vs Video Element
- **YouTube/Vimeo**: Uses `<iframe>` for embedded player
- **Direct URLs**: Uses `<video>` element with native controls
- Both support smooth transitions and autoplay

### Autoplay Considerations
- Videos start muted for browser autoplay policy compliance
- YouTube/Vimeo embeds include `mute=1` parameter
- Direct videos use `muted` attribute
- Users can unmute direct videos with mute button
