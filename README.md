# 🎬 WatchIt - Netflix Clone

<div align="center">

![WatchIt Banner](https://img.shields.io/badge/WatchIt-Streaming_Platform-E50914?style=for-the-badge&logo=netflix&logoColor=white)

**A modern, feature-rich streaming platform built with ASP.NET Core MVC**

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![Entity Framework](https://img.shields.io/badge/Entity_Framework-Core-512BD4?style=flat-square)](https://docs.microsoft.com/en-us/ef/)
[![SQL Server](https://img.shields.io/badge/SQL_Server-Database-CC2927?style=flat-square&logo=microsoft-sql-server)](https://www.microsoft.com/sql-server)
[![License](https://img.shields.io/badge/License-MIT-E50914?style=flat-square)](LICENSE)

[Features](#-features) • [Demo](#-demo) • [Installation](#-installation) • [Usage](#-usage) • [Tech Stack](#-tech-stack) • [Contributing](#-contributing)

</div>

---

## 🌟 Overview

**WatchIt** is a fully-functional Netflix-inspired streaming platform that delivers a premium viewing experience. Built with modern web technologies, it features a sleek, responsive UI with smooth animations, intelligent content recommendations, and comprehensive user management.

### ✨ Why WatchIt?

- 🎯 **Netflix-Quality UI** - Pixel-perfect design with smooth transitions and animations
- 🎥 **Smart Trailers** - YouTube/Vimeo integration with custom controls in hero banners
- 👤 **Multi-Profile Support** - Family-friendly with individual profiles and watch history
- 🔐 **Secure Authentication** - ASP.NET Identity with role-based access control
- 📱 **Fully Responsive** - Seamless experience across all devices
- 🎨 **Modern Design** - Dark theme with glassmorphism effects

---

## 🚀 Features

### 🎬 Content Management

<table>
<tr>
<td width="50%">

#### Movies & Series
- ✅ Comprehensive movie library
- ✅ TV series with seasons & episodes
- ✅ Series of movies (franchises)
- ✅ Rich metadata (cast, categories, ratings)
- ✅ High-quality poster images
- ✅ Trailer integration (YouTube/Vimeo)

</td>
<td width="50%">

#### Smart Organization
- ✅ Category-based browsing
- ✅ Genre filtering
- ✅ Rating system (0-10)
- ✅ Duration tracking
- ✅ Age ratings (18+ filtering)
- ✅ Search functionality

</td>
</tr>
</table>

### 🎭 User Experience

<table>
<tr>
<td width="50%">

#### Personalization
- 👤 Multiple user profiles
- 📺 Continue watching
- ❤️ My List (favorites)
- 📊 Watch history tracking
- 🎯 Personalized recommendations
- 🔄 Resume playback

</td>
<td width="50%">

#### Interactive Features
- ▶️ Trailer playback in hero banners
- 🔊 Custom audio controls
- 🎬 Smooth video transitions
- 📱 Responsive card hover effects
- 🎨 Glassmorphism UI elements
- ⚡ Fast, smooth animations

</td>
</tr>
</table>

### 🎥 Hero Trailer System

Our **signature feature** - immersive hero banners with integrated trailer playback:

- 🎬 **Auto-detecting video sources** (YouTube, Vimeo, Direct MP4)
- ▶️ **Centered play button** with smooth scale animations
- 🔊 **Custom speaker controls** for all video types
- 🎭 **Seamless transitions** from poster to video
- 🔁 **Auto-looping** trailers
- 📱 **Mobile-optimized** controls

### 🔐 Authentication & Authorization

- 🔑 ASP.NET Identity integration
- 👥 Role-based access (Admin/User)
- 🔒 Secure login/registration
- 📧 Email confirmation support
- 🔐 Password recovery
- 👤 Profile management

### 🎨 UI/UX Highlights

- 🌑 **Dark Theme** - Easy on the eyes
- 🎨 **Brand Colors** - Netflix-inspired red (#E50914)
- ✨ **Glassmorphism** - Modern frosted glass effects
- 🎭 **Smooth Animations** - 60fps transitions
- 📱 **Responsive Design** - Mobile-first approach
- 🎯 **Intuitive Navigation** - Easy content discovery

---

## 📸 Demo

### 🏠 Home Page
- **Hero Banner** with trailer playback
- **For You** - Personalized recommendations
- **Continue Watching** - Resume where you left off
- **Trending Now** - Popular content
- **Top 10** - Most-watched content
- **Category Rows** - Action, Drama, Comedy, etc.

### 🎬 Movie/Series Details
- **Immersive Hero** with trailer integration
- **Full Metadata** - Cast, genres, ratings
- **Video Player** - Resume functionality
- **Seasons & Episodes** - For TV series
- **Quick Actions** - Add to list, edit (admin)

### 👤 User Profiles
- **Profile Selection** - Family-friendly
- **Watch History** - Track progress
- **My List** - Save favorites
- **Profile Settings** - Customize experience

---

## 🛠️ Tech Stack

### Backend
```
🔹 ASP.NET Core 10.0 MVC
🔹 Entity Framework Core
🔹 SQL Server
🔹 ASP.NET Identity
🔹 LINQ
```

### Frontend
```
🔹 Razor Views
🔹 HTML5 / CSS3
🔹 JavaScript (ES6+)
🔹 YouTube IFrame API
🔹 Vimeo Player API
```

### Design
```
🔹 Custom CSS (No frameworks)
🔹 CSS Grid & Flexbox
🔹 CSS Animations
🔹 Glassmorphism Effects
🔹 Responsive Design
```

---

## 📦 Installation

### Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/sql-server) (LocalDB or Express)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

### Setup Steps

1️⃣ **Clone the repository**
```bash
git clone https://github.com/yourusername/watchit-netflix-clone.git
cd watchit-netflix-clone
```

2️⃣ **Configure database connection**

Update your connection string in `appsettings.json` to match your SQL Server setup.

3️⃣ **Apply migrations**
```bash
dotnet ef database update
```

4️⃣ **Run the application**
```bash
dotnet run
```

5️⃣ **Open in browser**
```
https://localhost:5155
```

---

## 🎯 Usage

### Admin Features

Create an admin account through the registration system and assign admin role via database.

**Admin Capabilities:**
- ➕ Add/Edit/Delete Movies & Series
- 🎭 Manage Actors & Categories
- 👥 User Management
- 📊 Content Analytics

### User Features

**Registration:**
1. Click "Sign Up" in navbar
2. Fill in registration form
3. Confirm email (if enabled)
4. Create user profile

**Watching Content:**
1. Browse home page or categories
2. Click on movie/series card
3. Click play button in hero banner (trailer)
4. Scroll down to watch full content
5. Progress is automatically saved

**Managing Favorites:**
1. Click "Add to My List" on any content
2. Access "My List" from navbar
3. Remove items anytime

---

## 🎨 Color Palette

Our brand colors inspired by Netflix:

| Color | Hex | Usage |
|-------|-----|-------|
| 🔴 **Brand Red** | `#E50914` | Primary actions, highlights |
| ⚫ **Void Black** | `#080810` | Background base |
| 🌑 **Dark Surface** | `#0D0D18` | Cards, surfaces |
| 🌫️ **Elevated** | `#1A1A2E` | Elevated elements |
| ⚪ **Text Primary** | `#F0F0F8` | Main text |
| 🌫️ **Text Secondary** | `#9898B8` | Secondary text |

---

## 📁 Project Structure

```
Netflix-clone/
├── 📂 Controllers/          # MVC Controllers
│   ├── HomeController.cs
│   ├── MovieController.cs
│   ├── SeriesController.cs
│   └── ...
├── 📂 Models/              # Data Models
│   ├── Movie.cs
│   ├── Series.cs
│   ├── Actor.cs
│   └── ...
├── 📂 Views/               # Razor Views
│   ├── Home/
│   ├── Movie/
│   ├── Series/
│   └── Shared/
├── 📂 Areas/               # Identity Area
│   └── Identity/
├── 📂 Migrations/          # EF Migrations
├── 📂 wwwroot/            # Static Files
│   ├── css/
│   ├── js/
│   └── images/
└── 📄 Program.cs          # App Entry Point
```

---

## 🔑 Key Features Explained

### 🎬 Hero Trailer System

The hero trailer system automatically detects video sources and provides seamless playback:

**Supported Formats:**
- YouTube URLs (`youtube.com`, `youtu.be`)
- Vimeo URLs (`vimeo.com`)
- Direct MP4 files

**Features:**
- Automatic video ID extraction
- Optimized embed parameters
- Custom audio controls via postMessage API
- Smooth fade transitions
- Auto-loop functionality

### 📊 Watch History

Tracks user viewing progress with:
- Automatic progress saving (every 15 seconds)
- Resume functionality
- Progress percentage calculation
- Last watched timestamp
- Beacon API for reliable saving

### 👤 Multi-Profile System

Family-friendly profile management:
- Multiple profiles per account
- Individual watch history
- Separate "My List" per profile
- Profile-specific recommendations
- Kid-friendly profiles

---

## 🚧 Roadmap

### Planned Features

- [ ] 🔍 Advanced search with filters
- [ ] 🎯 AI-powered recommendations
- [ ] 💳 Subscription management
- [ ] 📱 Mobile apps (iOS/Android)
- [ ] 🌐 Multi-language support
- [ ] 📧 Email notifications
- [ ] 🎮 Gamification (achievements)
- [ ] 📊 Admin analytics dashboard
- [ ] 🎬 Live streaming support
- [ ] 💬 Comments & reviews

---

## 🤝 Contributing

We welcome contributions! Here's how you can help:

1. 🍴 Fork the repository
2. 🌿 Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. 💾 Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. 📤 Push to the branch (`git push origin feature/AmazingFeature`)
5. 🔃 Open a Pull Request

### Contribution Guidelines

- Follow existing code style
- Write meaningful commit messages
- Add comments for complex logic
- Test thoroughly before submitting
- Update documentation as needed

---

<div align="center">

### ⭐ Star this repo if you find it useful!

**Made with ❤️ and lots of ☕**

![WatchIt](https://img.shields.io/badge/WatchIt-Stream_Anywhere-E50914?style=for-the-badge&logo=netflix&logoColor=white)

</div>
