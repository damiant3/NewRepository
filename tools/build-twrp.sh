#!/usr/bin/env bash
# Build TWRP recovery.img for Samsung Galaxy S7 Edge (SM-G935T / hero2qlte)
# Run inside WSL: wsl -d Ubuntu-24.04 -- bash /mnt/d/Projects/NewRepository/tools/build-twrp.sh
#
# Prerequisites installed by this script. Requires ~15GB disk for repo sync + build.
# Output: ~/twrp-hero2qlte/out/target/product/hero2qlte/recovery.img

set -euo pipefail

BUILD_DIR="$HOME/twrp-hero2qlte"
JOBS=$(nproc)

echo "=== TWRP Build for hero2qlte (SM-G935T) ==="
echo "Build dir: $BUILD_DIR"
echo "Cores: $JOBS"
echo ""

# ── Step 1: Install AOSP build dependencies ──────────────────────────────────
echo ">>> Step 1: Installing build dependencies..."
sudo apt-get update -qq
sudo apt-get install -y -qq \
    bc bison build-essential ccache curl flex g++-multilib gcc-multilib \
    git gnupg gperf imagemagick lib32ncurses5-dev lib32readline-dev \
    lib32z1-dev liblz4-tool libncurses5 libncurses5-dev libsdl1.2-dev \
    libssl-dev libxml2 libxml2-utils lzop pngcrush rsync schedtool \
    squashfs-tools xsltproc zip zlib1g-dev \
    openjdk-11-jdk python3 python-is-python3 \
    2>/dev/null
echo "    Dependencies installed."

# ── Step 2: Install repo tool ────────────────────────────────────────────────
echo ">>> Step 2: Installing repo tool..."
mkdir -p ~/bin
if [ ! -f ~/bin/repo ]; then
    curl -s https://storage.googleapis.com/git-repo-downloads/repo > ~/bin/repo
    chmod a+x ~/bin/repo
fi
export PATH=~/bin:$PATH
echo "    repo installed at ~/bin/repo"

# ── Step 3: Configure git (required by repo) ─────────────────────────────────
echo ">>> Step 3: Configuring git..."
git config --global user.email "build@codex.local" 2>/dev/null || true
git config --global user.name "TWRP Build" 2>/dev/null || true
git config --global color.ui false 2>/dev/null || true

# ── Step 4: Init repo with TWRP minimal manifest ─────────────────────────────
echo ">>> Step 4: Initializing TWRP manifest..."
mkdir -p "$BUILD_DIR"
cd "$BUILD_DIR"

if [ ! -d ".repo" ]; then
    # Try twrp-6.0 first (matches jcadduono's device tree branch)
    # Fall back to twrp-7.1 if 6.0 doesn't exist
    if repo init -u https://github.com/minimal-manifest-twrp/platform_manifest_twrp_aosp.git -b twrp-6.0 --depth=1 2>/dev/null; then
        echo "    Initialized with twrp-6.0 manifest"
    else
        echo "    twrp-6.0 not available, trying twrp-7.1..."
        repo init -u https://github.com/minimal-manifest-twrp/platform_manifest_twrp_aosp.git -b twrp-7.1 --depth=1
        echo "    Initialized with twrp-7.1 manifest"
    fi
else
    echo "    .repo already exists, skipping init"
fi

# ── Step 5: Add device tree + kernel to local manifests ───────────────────────
echo ">>> Step 5: Adding hero2qlte device tree + kernel..."
mkdir -p .repo/local_manifests
cat > .repo/local_manifests/hero2qlte.xml << 'MANIFEST_EOF'
<?xml version="1.0" encoding="UTF-8"?>
<manifest>
  <remote name="jcadduono" fetch="https://github.com/jcadduono" />
  <project path="device/samsung/hero2qlte"
           name="android_device_samsung_hero2qlte"
           remote="jcadduono"
           revision="android-6.0" />
  <project path="kernel/samsung/msm8996"
           name="android_kernel_samsung_msm8996"
           remote="jcadduono"
           revision="twrp-6.0" />
</manifest>
MANIFEST_EOF
echo "    Local manifest written."

# ── Step 6: Repo sync ────────────────────────────────────────────────────────
echo ">>> Step 6: Syncing (this will download ~10-15GB)..."
repo sync -j"$JOBS" --force-sync --no-clone-bundle --no-tags
echo "    Sync complete."

# ── Step 7: Check for omni.dependencies ───────────────────────────────────────
echo ">>> Step 7: Checking device dependencies..."
if [ -f "device/samsung/hero2qlte/omni.dependencies" ]; then
    echo "    WARNING: omni.dependencies found — may need additional repos:"
    cat "device/samsung/hero2qlte/omni.dependencies"
    echo ""
    echo "    If build fails, add these repos to .repo/local_manifests/hero2qlte.xml"
fi

# ── Step 8: Build ─────────────────────────────────────────────────────────────
echo ">>> Step 8: Building TWRP recovery image..."
# shellcheck disable=SC1091
source build/envsetup.sh
lunch omni_hero2qlte-eng
make -j"$JOBS" recoveryimage

# ── Step 9: Check output ──────────────────────────────────────────────────────
RECOVERY_IMG="out/target/product/hero2qlte/recovery.img"
if [ -f "$RECOVERY_IMG" ]; then
    echo ""
    echo "============================================"
    echo "  SUCCESS: recovery.img built!"
    echo "  Location: $BUILD_DIR/$RECOVERY_IMG"
    echo "  Size: $(du -h "$RECOVERY_IMG" | cut -f1)"
    echo ""
    echo "  Next steps:"
    echo "    1. Copy to Windows:"
    echo "       cp $BUILD_DIR/$RECOVERY_IMG /mnt/d/Projects/recovery.img"
    echo "    2. Wrap for Odin:"
    echo "       tar -cf /mnt/d/Projects/recovery.img.tar -C $BUILD_DIR/out/target/product/hero2qlte recovery.img"
    echo "    3. Flash via Odin: AP slot -> recovery.img.tar -> Start"
    echo "============================================"

    # Auto-copy to Windows project directory
    cp "$RECOVERY_IMG" /mnt/d/Projects/recovery.img
    tar -cf /mnt/d/Projects/recovery.img.tar -C "$(dirname "$BUILD_DIR/$RECOVERY_IMG")" recovery.img
    echo "  Copied recovery.img and recovery.img.tar to D:\\Projects\\"
else
    echo ""
    echo "============================================"
    echo "  FAILED: recovery.img not found at expected path"
    echo "  Check build output above for errors."
    echo "  Common issues:"
    echo "    - Missing dependencies in omni.dependencies"
    echo "    - Wrong manifest branch (try twrp-7.1 or twrp-9.0)"
    echo "    - Kernel config issues"
    echo "============================================"
    exit 1
fi
