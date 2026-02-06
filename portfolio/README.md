# Marcel Schneider - Developer Portfolio

A stunning, modern, and responsive single-page portfolio website showcasing the work and skills of Marcel Schneider, Software Engineer.

![Portfolio Preview](https://via.placeholder.com/1200x630/3b82f6/ffffff?text=Marcel+Schneider+Portfolio)

## 🌟 Live Demo

**[View Portfolio](#)** *(Deploy using the Publish tab)*

## 📋 Table of Contents

- [About](#about)
- [Features](#features)
- [Technologies Used](#technologies-used)
- [Project Structure](#project-structure)
- [Sections](#sections)
- [Getting Started](#getting-started)
- [Customization Guide](#customization-guide)
- [Browser Support](#browser-support)
- [Performance](#performance)
- [License](#license)
- [Contact](#contact)

## 📖 About

This portfolio website serves as a comprehensive digital showcase for Marcel Schneider, highlighting:
- Professional experience as a Software Engineer
- Technical expertise in C#, WPF, C++
- Open-source projects including KataWPF (C# practice) and Heliconia (C++ helloworld)
- Creative side with music production tutorials on YouTube
- Passion for travel, music, and hardware engineering

## ✨ Features

### 🎨 Design & UI
- **Modern, Clean Design** - Professional aesthetic with attention to detail
- **Dark/Light Mode Toggle** - Seamless theme switching with localStorage persistence
- **Responsive Layout** - Mobile-first design that works on all devices
- **Smooth Animations** - Subtle fade-in effects and scroll-triggered animations
- **Interactive Elements** - Hover effects, transitions, and micro-interactions
- **Gradient Accents** - Eye-catching color gradients throughout

### 🚀 Performance
- **Optimized Loading** - Fast page load times with minimal dependencies
- **Lazy Loading** - Images and content load on demand
- **Smooth Scrolling** - Native smooth scroll behavior
- **Debounced/Throttled Events** - Optimized scroll and resize handlers

### ♿ Accessibility
- **Semantic HTML** - Proper heading hierarchy and semantic elements
- **ARIA Labels** - Screen reader friendly
- **Keyboard Navigation** - Full keyboard support (Tab, Enter, Escape, T for theme toggle)
- **Focus Management** - Clear focus indicators and focus trapping for mobile menu
- **Color Contrast** - WCAG AA compliant color combinations

### 📱 Responsive
- **Desktop** (1024px+) - Full multi-column layout
- **Tablet** (768px - 1023px) - Adapted two-column layout
- **Mobile** (< 768px) - Single column with hamburger menu

## 🛠 Technologies Used

### Core Technologies
- **HTML5** - Semantic markup
- **CSS3** - Modern styling with CSS Variables
- **JavaScript (ES6+)** - Vanilla JS for interactivity

### Libraries & Tools (via CDN)
- **Google Fonts** - Inter & Fira Code typefaces
- **Font Awesome 6.4.0** - Icon library
- **Native APIs** - Intersection Observer, Local Storage

### Build Tools
- None required! This is a static website ready to deploy

## 📁 Project Structure

```
portfolio/
├── index.html          # Main HTML file
├── css/
│   └── style.css      # All styles (28KB)
├── js/
│   └── script.js      # All JavaScript (13KB)
└── README.md          # This file
```

## 🎯 Sections

### 1. **Hero Section**
- Eye-catching introduction with animated code window
- Professional title and description
- Call-to-action buttons
- Social media links (GitHub, LinkedIn, YouTube)
- Animated scroll indicator

### 2. **About Section**
- Comprehensive background and experience
- Current role at Digitec Galaxus AG
- Skills and interests highlights
- Animated statistics cards
- Personal passions (travel, music)

### 3. **Skills Section**
- **Programming Languages**: C#, C++, Python, JavaScript
- **Frameworks & UI**: WPF, .NET, Qt, XAML
- **Tools & DevOps**: Git, GitHub Actions, CMake, Docker, CI/CD
- **Creative**: Cakewalk, Music Production, Audio Engineering
- **Professional**: Problem Solving, Team Collaboration, Documentation

### 4. **Projects Section**

#### Featured Projects

**KataWPF** - C# Dojo
- WPF coding exercises
- MVVM architecture
- Clean code practices
- Desktop application development
- [View on GitHub](https://github.com/marcello-s/dojo_csharp/tree/main/KataWPF)

**Heliconia** - C++ helloworld
- CI/CD pipeline with GitHub Actions
- CMake build system
- Comprehensive unit testing
- [View on GitHub](https://github.com/marcello-s/Heliconia)

### 5. **Media Section**
- YouTube channel showcase (@marcellos5778)
- Featured video tutorials:
  - Cakewalk - Exporting & Mixing
  - Combining & Splitting Midi-Tracks
  - Midi Chord Analyzer
  - Markers and Sections
- "Making Music step-by-step" playlist
- Music production content

### 6. **Contact Section**
- Multiple contact methods
- LinkedIn, GitHub, YouTube links
- Contact form placeholder (backend coming soon)
- Professional call-to-action

### 7. **Footer**
- Quick navigation links
- Social media links
- Interests and areas of expertise
- Copyright information

## 🚀 Getting Started

### Option 1: Direct Deployment
1. Click the **Publish** tab in your development environment
2. Deploy with one click
3. Share your live portfolio URL!

### Option 2: Local Development
1. Download all files maintaining the folder structure
2. Open `index.html` in a modern web browser
3. No build process or server required!

### Option 3: GitHub Pages
1. Create a GitHub repository
2. Upload all files
3. Enable GitHub Pages in repository settings
4. Your site will be live at `https://username.github.io/repository-name/`

## 🎨 Customization Guide

### Updating Personal Information

#### 1. Replace Placeholder Email
In `index.html`, find and update:
```html
<p class="placeholder-text">[Your email here]</p>
```

#### 2. Update Current Year
The footer year is automatically updated via JavaScript, but you can manually set it in:
```html
<p>&copy; 2026 Marcel Schneider...</p>
```

#### 3. Add Real Project Images
Currently using gradient backgrounds. To add project images:
```html
<div class="project-image" style="background-image: url('path/to/image.jpg');">
```

#### 4. Update Stats/Metrics
In the About section, update the stat cards with real numbers:
```html
<div class="stat-card">
    <h3>50+</h3> <!-- Update this -->
    <p>Projects Completed</p>
</div>
```

### Theme Customization

All colors and styles are controlled via CSS Variables in `:root`:

```css
:root {
    --accent-primary: #3b82f6;      /* Primary blue */
    --accent-secondary: #8b5cf6;    /* Secondary purple */
    --accent-gradient: linear-gradient(135deg, #3b82f6 0%, #8b5cf6 100%);
    /* ... more variables ... */
}
```

### Adding New Sections

1. Add HTML in `index.html`:
```html
<section id="new-section" class="new-section section">
    <div class="container">
        <!-- Your content -->
    </div>
</section>
```

2. Add navigation link:
```html
<li><a href="#new-section" class="nav-link">New Section</a></li>
```

3. Add styles in `css/style.css`:
```css
.new-section {
    /* Your styles */
}
```

## 🌐 Browser Support

- ✅ Chrome/Edge (90+)
- ✅ Firefox (88+)
- ✅ Safari (14+)
- ✅ Opera (76+)

### Required Browser Features
- CSS Variables (Custom Properties)
- CSS Grid & Flexbox
- Intersection Observer API
- Local Storage API
- ES6+ JavaScript

## ⚡ Performance

### Current Metrics
- **HTML**: ~34KB
- **CSS**: ~28KB
- **JavaScript**: ~13KB
- **Total (uncompressed)**: ~75KB
- **External Assets**: Google Fonts + Font Awesome (cached by CDN)

### Optimization Tips
1. **Enable GZIP compression** on your server (reduces size by ~70%)
2. **Use CDN** for external libraries (already implemented)
3. **Optimize images** if you add them (WebP format recommended)
4. **Enable browser caching** for static assets

## 📝 Current Status

### ✅ Completed Features
- [x] Responsive design for all screen sizes
- [x] Dark/light mode with localStorage persistence
- [x] Smooth scrolling and navigation
- [x] Animated elements on scroll
- [x] Mobile-friendly hamburger menu
- [x] Social media integration
- [x] Project showcases with GitHub links
- [x] YouTube channel integration
- [x] Skills categorization
- [x] About section with highlights
- [x] Contact methods display
- [x] Footer with navigation
- [x] Scroll-to-top button
- [x] Keyboard navigation support
- [x] Accessibility features

### 🚧 Planned Features / To-Do
- [ ] Add email address (currently placeholder)
- [ ] Implement backend for contact form
- [ ] Add real project images/screenshots
- [ ] Integrate blog section (optional)
- [ ] Add testimonials section (optional)
- [ ] Include downloadable resume/CV
- [ ] Add Google Analytics or privacy-friendly alternative
- [ ] Create project detail pages (optional)
- [ ] Add video embedding from YouTube
- [ ] Include certifications section (if applicable)

### 💡 Recommendations for Next Steps

1. **Add Professional Photo**: Replace code window with a professional headshot
2. **Enable Contact Form Backend**: Use services like Formspree, Netlify Forms, or EmailJS
3. **SEO Optimization**: Add meta tags, Open Graph tags, and structured data
4. **Analytics**: Track visitors with privacy-friendly analytics
5. **Blog Integration**: Share technical articles and tutorials
6. **Portfolio Expansion**: Add more projects as they're developed
7. **Performance Monitoring**: Use Lighthouse to track Core Web Vitals
8. **A/B Testing**: Test different CTAs and layouts
9. **Newsletter Signup**: Build an audience (optional)
10. **Multilingual Support**: Add German language option (for Swiss market)

## 📜 License

This portfolio website is open source and available for personal use. Feel free to use this as a template for your own portfolio!

**Attribution appreciated but not required.**

## 📞 Contact

**Marcel Schneider**
- 💼 LinkedIn: [marcel-schneider-05480512b](https://www.linkedin.com/in/marcel-schneider-05480512b/)
- 🐙 GitHub: [@marcello-s](https://github.com/marcello-s)
- 🎥 YouTube: [@marcellos5778](https://www.youtube.com/@marcellos5778)
- 📧 Email: *[Add your email here]*

---

## 🎓 Technical Notes

### CSS Architecture
- **CSS Variables** for theming and easy customization
- **Mobile-first approach** with progressive enhancement
- **BEM-inspired naming** for clarity
- **Utility classes** for common patterns
- **Smooth transitions** for theme switching

### JavaScript Features
- **ES6+ syntax** (arrow functions, const/let, template literals)
- **Event delegation** where appropriate
- **Debouncing/throttling** for performance
- **Intersection Observer** for scroll animations
- **Local Storage** for theme persistence
- **Console Easter eggs** for fellow developers 😉

### Accessibility Considerations
- Semantic HTML5 elements
- ARIA labels on interactive elements
- Focus management for modal/menu states
- Keyboard shortcuts (T = toggle theme, Esc = close menu)
- High contrast ratios in both themes
- Proper heading hierarchy

### Performance Optimizations
- Minimal external dependencies
- Throttled scroll event handlers
- Lazy loading support (ready for images)
- CSS-only animations where possible
- Efficient selector usage
- No render-blocking resources

---

**Built with ❤️ and code by Marcel Schneider**

*Last Updated: February 2026*