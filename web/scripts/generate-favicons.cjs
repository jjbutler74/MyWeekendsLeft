const sharp = require('sharp');
const fs = require('fs');
const path = require('path');

const publicDir = path.join(__dirname, '..', 'public');
const svgPath = path.join(publicDir, 'weekend.svg');
const svgBuffer = fs.readFileSync(svgPath);

const sizes = [
  { name: 'favicon-16x16.png', size: 16 },
  { name: 'favicon-32x32.png', size: 32 },
  { name: 'apple-touch-icon.png', size: 180 },
  { name: 'android-chrome-192x192.png', size: 192 },
  { name: 'android-chrome-512x512.png', size: 512 },
  { name: 'og-image-icon.png', size: 512 },
];

async function generate() {
  for (const { name, size } of sizes) {
    await sharp(svgBuffer)
      .resize(size, size)
      .png()
      .toFile(path.join(publicDir, name));
    console.log(`Generated ${name} (${size}x${size})`);
  }

  // Generate favicon.ico from 32x32 PNG
  // ICO format: just use the 32x32 PNG as a simple approach
  const png32 = await sharp(svgBuffer).resize(32, 32).png().toBuffer();
  const png16 = await sharp(svgBuffer).resize(16, 16).png().toBuffer();

  // Simple ICO file with one 32x32 image
  const ico = createIco([
    { buffer: png16, size: 16 },
    { buffer: png32, size: 32 },
  ]);
  fs.writeFileSync(path.join(publicDir, 'favicon.ico'), ico);
  console.log('Generated favicon.ico (16x16 + 32x32)');

  // Generate OG image (1200x630) with the icon centered on a gradient background
  const ogIcon = await sharp(svgBuffer).resize(300, 300).png().toBuffer();
  const ogImage = await sharp({
    create: {
      width: 1200,
      height: 630,
      channels: 4,
      background: { r: 255, g: 248, b: 240, alpha: 1 }, // cream background
    },
  })
    .composite([
      {
        input: ogIcon,
        top: 80,
        left: 450,
      },
      {
        input: Buffer.from(
          `<svg width="1200" height="630">
            <text x="600" y="460" font-family="Arial, sans-serif" font-size="48" font-weight="bold" fill="#2D3436" text-anchor="middle">MyWeekendsLeft</text>
            <text x="600" y="520" font-family="Arial, sans-serif" font-size="28" fill="#636e72" text-anchor="middle">Make Every Weekend Count</text>
          </svg>`
        ),
        top: 0,
        left: 0,
      },
    ])
    .png()
    .toFile(path.join(publicDir, 'og-image.png'));
  console.log('Generated og-image.png (1200x630)');

  // Create site.webmanifest
  const manifest = {
    name: 'MyWeekendsLeft',
    short_name: 'Weekends',
    icons: [
      { src: '/android-chrome-192x192.png', sizes: '192x192', type: 'image/png' },
      { src: '/android-chrome-512x512.png', sizes: '512x512', type: 'image/png' },
    ],
    theme_color: '#FF6B35',
    background_color: '#FFF8F0',
    display: 'standalone',
  };
  fs.writeFileSync(
    path.join(publicDir, 'site.webmanifest'),
    JSON.stringify(manifest, null, 2)
  );
  console.log('Generated site.webmanifest');
}

// Simple ICO format builder
function createIco(images) {
  const headerSize = 6;
  const dirEntrySize = 16;
  const numImages = images.length;
  let dataOffset = headerSize + dirEntrySize * numImages;

  const buffers = [Buffer.alloc(headerSize)];

  // ICO header
  buffers[0].writeUInt16LE(0, 0); // Reserved
  buffers[0].writeUInt16LE(1, 2); // Type: ICO
  buffers[0].writeUInt16LE(numImages, 4); // Number of images

  const dirEntries = [];
  const imageDataBuffers = [];

  for (const { buffer, size } of images) {
    const entry = Buffer.alloc(dirEntrySize);
    entry.writeUInt8(size === 256 ? 0 : size, 0); // Width
    entry.writeUInt8(size === 256 ? 0 : size, 1); // Height
    entry.writeUInt8(0, 2); // Color palette
    entry.writeUInt8(0, 3); // Reserved
    entry.writeUInt16LE(1, 4); // Color planes
    entry.writeUInt16LE(32, 6); // Bits per pixel
    entry.writeUInt32LE(buffer.length, 8); // Image size
    entry.writeUInt32LE(dataOffset, 12); // Data offset
    dirEntries.push(entry);
    imageDataBuffers.push(buffer);
    dataOffset += buffer.length;
  }

  return Buffer.concat([buffers[0], ...dirEntries, ...imageDataBuffers]);
}

generate().catch(console.error);
