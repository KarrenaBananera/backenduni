#!/bin/sh
set -eu

API_URL="${API_URL:-/api}"
export API_PROXY="${API_PROXY:-http://app:8080}"

printf '%s\n' "{\"apiUrl\":\"${API_URL}\"}" > /usr/share/nginx/html/config.json
envsubst '${API_PROXY}' < /etc/nginx/templates/default.conf.template > /etc/nginx/conf.d/default.conf

exec nginx -g 'daemon off;'
