#!/usr/bin/env bash
# TWRP Build Script for Samsung Galaxy S7 Edge (hero2qlte / SM-G935T)
# Run inside WSL Ubuntu 24.04
# Expected time: ~30-60 min (sync) + ~15-30 min (build)
# Expected disk: ~15-20 GB
set -euo pipefail

WORK_DIR="$HOME/twrp-hero2qlte"
OUTPUT_DIR="/mnt/d/Projects/NewRepository/phone"

echo "=== TWRP Build for hero2qlte ==="
echo "Work dir: $WORK_DIR"
echo "Output:   $OUTPUT_DIR"
echo ""

# ── Step 1: Install build dependencies ──────────────────────────
echo ">>> Step 1: Installing build dependencies..."
sudo apt-get update -qq
sudo apt-get install -y -qq \
    git gnupg flex bison gperf build-essential \
    zip curl zlib1g-dev gcc-multilib g++-multilib \
    libc6-dev-i386 lib32ncurses-dev x11proto-core-dev \
    libx11-dev lib32z1-dev libgl1-mesa-dev libxml2-utils \
    xsltproc unzip python3 python-is-python3 \
    openjdk-11-jdk bc lzop pngcrush schedtool \
    libssl-dev rsync 2>&1 | tail -3
echo "    Build deps installed."

# ── Step 2: Install repo tool ───────────────────────────────────
echo ">>> Step 2: Installing repo tool..."
mkdir -p ~/bin
if [ ! -f ~/bin/repo ]; then
    curl -s https://storage.googleapis.com/git-repo-downloads/repo > ~/bin/repo
    chmod a+x ~/bin/repo
fi
export PATH=~/bin:$PATH
echo "    repo installed at ~/bin/repo"

# Configure git if not set
git config --global user.email >/dev/null 2>&1 || git config --global user.email "build@codex.local"
git config --global user.name >/dev/null 2>&1 || git config --global user.name "Codex Build"
git config --global color.ui false 2>/dev/null || true

# ── Step 3: Init TWRP manifest ─────────────────────────────────
echo ">>> Step 3: Initializing TWRP manifest..."
mkdir -p "$WORK_DIR"
cd "$WORK_DIR"

if [ ! -d .repo ]; then
    # Try twrp-6.0 first (matches device tree branch)
    # If that fails, fall back to twrp-7.1
    if ! repo init -u https://github.com/minimal-manifest-twrp/platform_manifest_twrp_aosp.git -b twrp-6.0 --depth=1 2>/dev/null; then
        echo "    twrp-6.0 branch not found, trying twrp-7.1..."
        repo init -u https://github.com/minimal-manifest-twrp/platform_manifest_twrp_aosp.git -b twrp-7.1 --depth=1
    fi
    echo "    Manifest initialized."
else
    echo "    Manifest already initialized, skipping."
fi

# ── Step 4: Add device tree via local manifest ──────────────────
echo ">>> Step 4: Adding hero2qlte device tree..."
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

# ── Step 5: Sync ───────────────────────────────────────────────
echo ">>> Step 5: Syncing sources (this takes a while)..."
repo sync -j$(nproc) --force-sync --no-clone-bundle --no-tags -c 2>&1 | tail -5
echo "    Sync complete."

# ── Step 6: Check for omni.dependencies ────────────────────────
echo ">>> Step 6: Checking device dependencies..."
if [ -f device/samsung/hero2qlte/omni.dependencies ]; then
    echo "    omni.dependencies found:"
    cat device/samsung/hero2qlte/omni.dependencies
    echo ""
    echo "    WARNING: Additional repos may be needed. Check above and add to local manifest if build fails."
else
    echo "    No omni.dependencies file — proceeding."
fi

# ── Step 7: Build ──────────────────────────────────────────────
echo ">>> Step 7: Building TWRP recovery image..."
cd "$WORK_DIR"

# shellcheck disable=SC1091
. build/envsetup.sh

lunch omni_hero2qlte-eng

make -j$(nproc) recoveryimage 2>&1 | tail -20

# ── Step 8: Copy output ────────────────────────────────────────
RECOVERY_IMG="out/target/product/hero2qlte/recovery.img"
if [ -f "$RECOVERY_IMG" ]; then
    mkdir -p "$OUTPUT_DIR"
    cp "$RECOVERY_IMG" "$OUTPUT_DIR/recovery.img"

    # Also create Odin-flashable tar
    cd out/target/product/hero2qlte/
    tar -cf recovery.img.tar recovery.img
    cp recovery.img.tar "$OUTPUT_DIR/recovery.img.tar"
    cd "$WORK_DIR"

    echo ""
    echo "=== BUILD SUCCESS ==="
    echo "  recovery.img:     $OUTPUT_DIR/recovery.img"
    echo "  recovery.img.tar: $OUTPUT_DIR/recovery.img.tar (Odin-ready)"
    echo ""
    echo "Next steps:"
    echo "  1. Open Odin on Windows"
    echo "  2. Put phone in Download Mode (Vol Down + Home + Power)"
    echo "  3. AP slot -> $OUTPUT_DIR/recovery.img.tar"
    echo "  4. Click Start"
    echo "  5. Boot into TWRP: Vol Up + Home + Power"
    echo ""
    ls -lh "$OUTPUT_DIR/recovery.img" "$OUTPUT_DIR/recovery.img.tar"
else
    echo ""
    echo "=== BUILD FAILED ==="
    echo "  recovery.img not found at $RECOVERY_IMG"
    echo "  Check build output above for errors."
    echo "  Common issues:"
    echo "    - Missing dependencies in omni.dependencies"
    echo "    - Manifest branch mismatch (try twrp-7.1 or twrp-9.0)"
    echo "    - Kernel config issues"
    exit 1
fi
