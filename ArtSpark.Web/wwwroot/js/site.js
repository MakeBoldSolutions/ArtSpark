// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

window.ArtSparkImages = {
  upgrade(root = document) {
    root.querySelectorAll("img.artwork-progressive-image[data-full-src]").forEach((img) => {
      const fullSrc = getBrowserImageUrl(img.dataset.fullSrc);
      if (!fullSrc || img.dataset.fullSrcLoaded === "true") {
        return;
      }

      img.dataset.fullSrcLoaded = "true";

      const fullImage = new Image();
      fullImage.onload = () => {
        img.src = fullSrc;
        img.classList.remove("artwork-image-fallback");
      };
      fullImage.onerror = () => {
        img.classList.add("artwork-image-fallback");
      };
      fullImage.src = fullSrc;
    });
  }
};

function getBrowserImageUrl(src) {
  if (!src) {
    return "";
  }

  return src.replace(
    /^\/api\/media\/images\/iiif\//,
    "https://www.artic.edu/iiif/2/"
  );
}

document.addEventListener("DOMContentLoaded", () => {
  window.ArtSparkImages.upgrade();
});
