#!/usr/bin/env bash
host="$1"
port="$2"
shift 2

echo "⏳ Waiting for $host:$port..."

until nc -z "$host" "$port"; do
  sleep 1
done

echo "✅ $host:$port is available - launching app..."
exec "$@"
