import os
from http.server import BaseHTTPRequestHandler, ThreadingHTTPServer
from pathlib import Path
from urllib.parse import unquote, urlparse

def required(name):
    value = os.environ.get(name, "").strip()
    if not value:
        raise SystemExit(f"{name} is required")
    return value


def prefix(value):
    if not value.startswith("/"):
        value = "/" + value
    if not value.endswith("/"):
        value += "/"
    return value


ROOT = Path(os.environ.get("DATA_ROOT", "/data"))
KEY = required("SERVICE_ROLE_KEY")
PORT = int(os.environ.get("PORT", "5000"))
OBJECT_PREFIX = prefix(required("OBJECT_PREFIX"))
PUBLIC_PREFIX = prefix(required("PUBLIC_PREFIX"))


class Handler(BaseHTTPRequestHandler):
    def do_OPTIONS(self):
        self.send_response(204)
        self.send_header("Access-Control-Allow-Origin", "*")
        self.send_header("Access-Control-Allow-Headers", "authorization, apikey, content-type, cache-control, x-upsert")
        self.send_header("Access-Control-Allow-Methods", "GET, POST, OPTIONS")
        self.end_headers()

    def do_POST(self):
        if not self._authorized():
            self._json(401, b'{"message":"Invalid API key"}')
            return

        relative = self._object_path()
        if relative is None or relative.startswith("public/"):
            self._json(404, b'{"message":"Object path was not found"}')
            return

        destination = self._safe_path(relative)
        if destination is None:
            self._json(400, b'{"message":"Invalid object path"}')
            return

        length = int(self.headers.get("Content-Length", "0"))
        body = self.rfile.read(length)
        destination.parent.mkdir(parents=True, exist_ok=True)
        destination.write_bytes(body)
        payload = ('{"Key":"%s"}' % relative).encode()
        self._json(200, payload)

    def do_GET(self):
        path = unquote(urlparse(self.path).path)
        if not path.startswith(PUBLIC_PREFIX):
            self._json(404, b'{"message":"Object path was not found"}')
            return

        relative = path[len(PUBLIC_PREFIX):]
        file_path = self._safe_path(relative)
        if file_path is None or not file_path.is_file():
            self._json(404, b'{"message":"Object not found"}')
            return

        content_type = {
            ".jpg": "image/jpeg",
            ".jpeg": "image/jpeg",
            ".png": "image/png",
            ".webp": "image/webp",
        }.get(file_path.suffix.lower(), "application/octet-stream")
        data = file_path.read_bytes()
        self.send_response(200)
        self.send_header("Content-Type", content_type)
        self.send_header("Content-Length", str(len(data)))
        self.send_header("Access-Control-Allow-Origin", "*")
        self.end_headers()
        self.wfile.write(data)

    def _authorized(self):
        authorization = self.headers.get("Authorization", "")
        api_key = self.headers.get("apikey", "")
        return authorization == f"Bearer {KEY}" or api_key == KEY

    def _object_path(self):
        path = unquote(urlparse(self.path).path)
        if not path.startswith(OBJECT_PREFIX):
            return None
        return path[len(OBJECT_PREFIX):]

    def _safe_path(self, relative):
        destination = (ROOT / relative).resolve()
        root = ROOT.resolve()
        if destination != root and root not in destination.parents:
            return None
        return destination

    def _json(self, status, payload):
        self.send_response(status)
        self.send_header("Content-Type", "application/json")
        self.send_header("Content-Length", str(len(payload)))
        self.end_headers()
        self.wfile.write(payload)

    def log_message(self, format, *args):
        return


if __name__ == "__main__":
    ROOT.mkdir(parents=True, exist_ok=True)
    ThreadingHTTPServer(("0.0.0.0", PORT), Handler).serve_forever()
