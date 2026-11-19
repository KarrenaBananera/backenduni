#!/bin/sh
set -eu

API_URL="${API_URL:-/api}"
export API_PROXY="${API_PROXY:-https://backenduni.onrender.com}"
export PORT="${PORT:-80}"

printf '%s\n' "{\"apiUrl\":\"${API_URL}\"}" > /usr/share/nginx/html/config.json
envsubst '${API_PROXY} ${PORT}' < /etc/nginx/templates/default.conf.template > /etc/nginx/conf.d/default.conf

exec nginx -g 'daemon off;'
