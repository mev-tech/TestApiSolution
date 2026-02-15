#!/usr/bin/env bash
# Creates GitHub rulesets for the TestApiSolution repository.
#
# Usage:
#   GITHUB_TOKEN=<your-pat> bash .github/scripts/create-rulesets.sh
#
# Required token scopes: repo (or fine-grained: Administration: write)

set -euo pipefail

OWNER="mev-tech"
REPO="TestApiSolution"
API="https://api.github.com/repos/${OWNER}/${REPO}/rulesets"

if [[ -z "${GITHUB_TOKEN:-}" ]]; then
  echo "Error: GITHUB_TOKEN is not set." >&2
  exit 1
fi

AUTH_HEADER="Authorization: Bearer ${GITHUB_TOKEN}"

create_ruleset() {
  local name="$1"
  local payload="$2"
  echo "Creating ruleset: ${name}"
  response=$(curl -sf -X POST "${API}" \
    -H "${AUTH_HEADER}" \
    -H "Accept: application/vnd.github+json" \
    -H "X-GitHub-Api-Version: 2022-11-28" \
    -d "${payload}")
  echo "  -> id=$(echo "${response}" | grep -o '"id":[0-9]*' | head -1 | cut -d: -f2)"
}

# ── 1. Master branch protection ───────────────────────────────────────────────
create_ruleset "master-protection" "$(cat <<'JSON'
{
  "name": "master-protection",
  "target": "branch",
  "enforcement": "active",
  "conditions": {
    "ref_name": {
      "include": ["refs/heads/master"],
      "exclude": []
    }
  },
  "rules": [
    { "type": "deletion" },
    { "type": "non_fast_forward" },
    {
      "type": "pull_request",
      "parameters": {
        "dismiss_stale_reviews_on_push": true,
        "require_code_owner_review": false,
        "required_approving_review_count": 1,
        "required_review_thread_resolution": false,
        "require_last_push_approval": false
      }
    },
    {
      "type": "required_status_checks",
      "parameters": {
        "strict_required_status_checks_policy": false,
        "required_status_checks": [
          { "context": "dotnet" },
          { "context": "docker" }
        ]
      }
    }
  ]
}
JSON
)"

# ── 2. Tag protection ─────────────────────────────────────────────────────────
create_ruleset "tag-protection" "$(cat <<'JSON'
{
  "name": "tag-protection",
  "target": "tag",
  "enforcement": "active",
  "conditions": {
    "ref_name": {
      "include": ["refs/tags/**"],
      "exclude": []
    }
  },
  "rules": [
    { "type": "deletion" },
    { "type": "non_fast_forward" },
    { "type": "creation" }
  ]
}
JSON
)"

echo ""
echo "Done. Verify at: https://github.com/${OWNER}/${REPO}/settings/rules"
