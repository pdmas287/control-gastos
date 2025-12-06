import hashlib
import base64

# Test password
password = "Admin123!"

# Generate SHA256 hash
hash_bytes = hashlib.sha256(password.encode('utf-8')).digest()
hash_base64 = base64.b64encode(hash_bytes).decode('utf-8')

print(f"Password: {password}")
print(f"SHA256 Hash (Base64): {hash_base64}")

# Compare with database hash
db_hash = "jGl25bVBBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrUSqRg="
print(f"\nDatabase Hash: {db_hash}")
print(f"Generated Hash: {hash_base64}")
print(f"Match: {db_hash == hash_base64}")
