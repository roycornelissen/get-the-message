terraform init -backend-config .\backend.conf -reconfigure -upgrade

terraform apply -var-file="demo.tfvars" -auto-approve

$connectionString = terraform output -raw servicebus_namespace_connection_string

$env:AzureServiceBus_Connectionstring = $connectionString

dotnet user-secrets set "ConnectionStrings:ServiceBus" $connectionString --project ..\src\AppHost\AppHost.csproj