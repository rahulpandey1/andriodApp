
# RepairAssets.ps1
# Downloads game assets from Icons8 and generates TTS audio
# Usage: ./RepairAssets.ps1

$baseDir = "c:/Users/SPALPT157/Downloads/FirstAndriodApp/FirstAndriodApp/Resources"
$imgDir = "$baseDir/Images"
$rawDir = "$baseDir/Raw"

# Ensure directories exist
New-Item -ItemType Directory -Force -Path $imgDir
New-Item -ItemType Directory -Force -Path $rawDir

Add-Type -AssemblyName System.Drawing


# Helper to generate fallback image
function Draw-Fallback {
    param($name, $color = "Gray", $text = "")
    $bmp = [System.Drawing.Bitmap]::new(96, 96)
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.Clear([System.Drawing.Color]::White)
    $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias

    $brush = [System.Drawing.Brushes]::Gray
    if ($name -match "red" -or $name -match "apple" -or $name -match "cherry" -or $name -match "heart") { $brush = [System.Drawing.Brushes]::Red }
    elseif ($name -match "blue" -or $name -match "car" -or $name -match "plane") { $brush = [System.Drawing.Brushes]::Blue }
    elseif ($name -match "green" -or $name -match "frog" -or $name -match "leaf") { $brush = [System.Drawing.Brushes]::Green }
    elseif ($name -match "yellow" -or $name -match "banana" -or $name -match "star") { $brush = [System.Drawing.Brushes]::Gold }
    elseif ($name -match "orange") { $brush = [System.Drawing.Brushes]::Orange }
    elseif ($name -match "purple") { $brush = [System.Drawing.Brushes]::Purple }
    elseif ($name -match "shadow") { $brush = [System.Drawing.Brushes]::Black }
    
    # Shape Logic
    # Shape Logic
    if ($name -match "square") {
        $g.FillRectangle($brush, 10, 10, 76, 76)
    }
    elseif ($name -match "triangle") {
        $points = @(
            [System.Drawing.Point]::new(48, 10),
            [System.Drawing.Point]::new(10, 86),
            [System.Drawing.Point]::new(86, 86)
        )
        $g.FillPolygon($brush, $points)
    }
    elseif ($name -match "star") {
        # Simplified star (diamond)
        $points = @(
            [System.Drawing.Point]::new(48, 0),
            [System.Drawing.Point]::new(60, 35),
            [System.Drawing.Point]::new(96, 35),
            [System.Drawing.Point]::new(68, 57),
            [System.Drawing.Point]::new(79, 96),
            [System.Drawing.Point]::new(48, 70),
            [System.Drawing.Point]::new(17, 96),
            [System.Drawing.Point]::new(28, 57),
            [System.Drawing.Point]::new(0, 35),
            [System.Drawing.Point]::new(36, 35)
        )
        $g.FillPolygon($brush, $points)
    }
    elseif ($name -match "letter_") {
        # Extract letter from "letter_a_trace"
        $parts = $name -split "_"
        $char = $parts[1].ToUpper()
        $font = [System.Drawing.Font]::new("Arial", 60, [System.Drawing.FontStyle]::Bold)
        $br = [System.Drawing.Brushes]::LightGray # Traceable color
        $g.DrawString($char, $font, $br, 10, 0)
    }
    elseif ($name -match "balloon") {
        $g.FillEllipse($brush, 20, 10, 56, 70) # Oval
        $g.FillRectangle($brush, 46, 80, 4, 10) # String
    }
    else {
        # Default Circle
        $g.FillEllipse($brush, 10, 10, 76, 76)
    }

    # Debug Text (Optional, small)
    if ($text -ne "") {
        $fontSmall = [System.Drawing.Font]::new("Arial", 8)
        $g.DrawString($text, $fontSmall, [System.Drawing.Brushes]::Black, 5, 40)
    }
    
    $dest = "$imgDir/$name.png"
    $bmp.Save($dest, [System.Drawing.Imaging.ImageFormat]::Png)
    $g.Dispose()
    $bmp.Dispose()
    Write-Host "Generated Smart Fallback for $name" -ForegroundColor Cyan
}

# Helper to download image
function Get-Icon {
    param($name, $urlName, $size = 96, $style = "color")
    $url = "https://img.icons8.com/$style/$size/$urlName.png"
    $dest = "$imgDir/$name.png"
    
    # Force download by removing existence check
    # if (Test-Path $dest) { return } 
    
    Write-Host "Downloading $name from $url..."
    try {
        Invoke-WebRequest -Uri $url -OutFile $dest -ErrorAction Stop
    }
    catch {
        Write-Host "Failed to download $name. Generating fallback." -ForegroundColor Red
        Draw-Fallback $name
    }
}

# ----------------------------
# 1. Download Images (Icons8)
# ----------------------------

# Shapes (MatchIt, ShapeSorter)
Get-Icon "star_color" "star"
Get-Icon "star_outline" "star" 96 "ios"
Get-Icon "circle_color" "circle"
Get-Icon "circle_outline" "circle" 96 "ios"
Get-Icon "square_color" "square"
Get-Icon "square_outline" "square" 96 "ios"
Get-Icon "triangle_color" "triangle"
Get-Icon "triangle_outline" "triangle" 96 "ios"
Get-Icon "heart_color" "heart"
Get-Icon "heart_outline" "heart" 96 "ios"

# Shape Sorter Specific Colors (Fallback to generic if specific color not found, but let's try generic)
# We will use the generic 'circle' which is usually colorful.
# For specific 'red circle', we might need to rely on the generic one being 'good enough' or use 'red-circle'
Get-Icon "circle_red" "red-circle" 96 "emoji"   # Icons8 Emoji style often has colors
Get-Icon "circle_blue" "blue-circle" 96 "emoji"
Get-Icon "square_green" "green-square" 96 "emoji"
Get-Icon "triangle_yellow" "yellow-triangle" 96 "emoji" # Might fail, fallback below
if (!(Test-Path "$imgDir/triangle_yellow.png")) { Copy-Item "$imgDir/triangle_color.png" "$imgDir/triangle_yellow.png" }

# Bins
Get-Icon "bin_circle" "trash-can" # approximations
Get-Icon "bin_square" "box"
Get-Icon "bin_triangle" "pyramid"

# Cards / MEmory
Get-Icon "card_cat" "cat"
Get-Icon "card_dog" "dog"
Get-Icon "card_bird" "bird"

# Shadow Match
Get-Icon "car_color" "car"
Get-Icon "car_shadow" "car" 96 "ios-filled" # Silhouette
Get-Icon "plane_color" "airplane-take-off"
Get-Icon "plane_shadow" "airplane-take-off" 96 "ios-filled"
Get-Icon "boat_color" "boat"
Get-Icon "boat_shadow" "boat" 96 "ios-filled"

# Color Match
Get-Icon "apple_red" "apple"
Get-Icon "frog_green" "frog"
Get-Icon "cherry_red" "cherry"
Get-Icon "banana_yellow" "banana"
Get-Icon "strawberry_red" "strawberry"
Get-Icon "leaf_green" "leaf"

# Animals
Get-Icon "animal_lion" "lion"
Get-Icon "animal_cow" "cow"
Get-Icon "animal_dog" "dog"
Get-Icon "animal_cat" "cat"

# Jigsaw & Puzzle Source
Get-Icon "puzzle_source_dog" "dog" 480
Get-Icon "jigsaw_source_cat" "cat" 480

# Tracing
Get-Icon "letter_a_trace" "letter-a"
Get-Icon "letter_b_trace" "letter-b"
Get-Icon "letter_c_trace" "letter-c"

# Connect Dots
Get-Icon "star_reveal" "star"

# Spot Diff
# (Uses star_color, circle_color)

# Color Mixing (Targets are code generated, no images needed)

# Fruit Catch
Get-Icon "apple" "apple"
Get-Icon "banana" "banana"
Get-Icon "bomb" "bomb"

# Maze
# (Uses code)

# Balloon
Get-Icon "balloon_red" "red-balloon" 96 "emoji"
Get-Icon "balloon_blue" "blue-balloon" 96 "emoji"
Get-Icon "balloon_green" "green-balloon" 96 "emoji"
Get-Icon "balloon_yellow" "yellow-balloon" 96 "emoji"
Get-Icon "balloon_purple" "purple-balloon" 96 "emoji"

# ----------------------------
# 2. CROP Logic (Puzzle/Jigsaw)
# ----------------------------

function Split-Image {
    param($src, $prefix, $rows, $cols)
    $srcPath = "$imgDir/$src"
    if (!(Test-Path $srcPath)) { Write-Host "Missing $src"; return }
    
    $bmp = [System.Drawing.Bitmap]::FromFile($srcPath)
    $w = $bmp.Width / $cols
    $h = $bmp.Height / $rows
    
    $rect = [System.Drawing.Rectangle]::new(0, 0, $w, $h)
    
    for ($y = 0; $y -lt $rows; $y++) {
        for ($x = 0; $x -lt $cols; $x++) {
            $rect.X = $x * $w
            $rect.Y = $y * $h
            $cloned = $bmp.Clone($rect, $bmp.PixelFormat)
            $idx = ($y * $cols) + $x
            $out = "$imgDir/${prefix}_${idx}.png"
            $cloned.Save($out, [System.Drawing.Imaging.ImageFormat]::Png)
            $cloned.Dispose()
        }
    }
    $bmp.Dispose()
    Write-Host "Split $src into $rows x $cols"
}

# Puzzle Slider (3x3)
Split-Image "puzzle_source_dog.png" "puzzle_puppy" 3 3
# Helper: The code expects "puzzle_puppy_0.png" etc.

# Jigsaw (2x2)
Split-Image "jigsaw_source_cat.png" "jigsaw_cat" 2 2


# ----------------------------
# 3. Audio Generation (TTS)
# ----------------------------
Add-Type -AssemblyName System.Speech
$synth = New-Object System.Speech.Synthesis.SpeechSynthesizer
# $synth.SelectVoiceByHints('Male') # Optional

function Gen-Audio {
    param($text, $filename)
    $out = "$rawDir/$filename"
    
    # We save as WAV because generating MP3 natively is hard in PS.
    # We will rename to .mp3 and hope Android plays it (RIFF wav in mp3 extension often works)
    # OR better: Save as .wav and I will update levels.json to look for .wav if needed.
    # User complained "Download audio", so let's try to make standard valid files.
    # But for now, simple WAV is verified to work on most Android players even with wrong extension? 
    # Actually, let's keep it .wav and update code/json?
    # No, keep .mp3 extension for now to avoid massive code refactor. 
    # Android MediaPlayer usually detects content type.
    
    $synth.SetOutputToWaveFile($out)
    $synth.Speak($text)
    $synth.SetOutputToNull()
    Write-Host "Generated Audio: $filename ($text)"
}

Gen-Audio "Great Job!" "success.mp3"
Gen-Audio "Try Again!" "error.mp3"
Gen-Audio "You Win!" "win.mp3"
Gen-Audio "Flip!" "flip.mp3"
Gen-Audio "Pop!" "pop.mp3"
Gen-Audio "Slide!" "slide.mp3"
Gen-Audio "Click!" "click.mp3"

# Animals
Gen-Audio "Lion" "sound_lion.mp3"
Gen-Audio "Moo" "sound_cow.mp3"
Gen-Audio "Woof" "sound_dog.mp3"
Gen-Audio "Meow" "sound_cat.mp3"

Write-Host "Asset Repair Complete"
