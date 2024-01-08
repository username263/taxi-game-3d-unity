import requests
from template_generator import generate_customer

temp_group = generate_customer('./Template(Dev).xlsx', 'Customer')
res = requests.put('https://localhost:7170/Template/Customer', json=temp_group, verify=False)

print(res.status_code)