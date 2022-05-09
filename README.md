# CDK for Terraform examples in C#
A collection of CDK for Terraform examples written in C# inspired by the official Terraform tutorial: https://learn.hashicorp.com/tutorials/terraform/cdktf?in=terraform/cdktf

HashiCorp Terraform allows you to define  your infrastructure projects using HashiCorp Configuration Language  (HCL). HCL is designed to make it easy for people to read and write  configuration that is simple and predictable. Some developers prefer to  use a familiar programming language to define their infrastructure,  however. This allows them to use the same language constructs and tools  they are familiar with for other development tasks. They can also use  all of the features and libraries available to fully featured  programming languages to develop complex infrastructure projects.

Cloud Development Kit for Terraform (CDKTF) allows you to define  infrastructure using your choice of C#, Python, TypeScript, or Java  (with experimental support for Go), while leveraging the hundreds of  providers and thousands of module definitions provided by Terraform and  the Terraform ecosystem.

By using your  programming language of choice, you can take advantage of the features  and development workflows you are familiar with. For example, when working with C#, you can use IDE features such as IntelliSense  in Visual Studio Code.

---

# Preparing your Cloud9 desktop

This section assumes you have followed the instructions to get an AWS account and have created a Cloud9 desktop.

### Increase development environment disk size

Cloud9 environments comes with 10 GB disk by default which is too small for our workshop. Increase the disk size to 50GB using the following process:

1. Go to the Cloud9 Environments page in the AWS Console [here](https://console.aws.amazon.com/cloud9/home) 

2. Select the environment you want to resize and click "View details" button.
3. EC2 Instance >> Go To Instance
4. Select the instance and go to the "Storage" tab. And then click the Volume ID.
5. Select the Volume and click "Actions" button on the top right side. Then select "Modify volume".
6. Resize the volume to 50 GB. Then refresh the "Volumes" page.
7. After you complete modifying process, go to the "Instances" page again and Reboot the instance.
8. Wait about two minutes and reopen the Cloud9 environment.


In the Cloud9 environment command line run df -h to verify the new 50 GB disk size.

### Install .net core 6.0 and 3.1

In the Cloud9 command line execute the following command to add the Microsoft package signing key to your list of trusted keys and add the Microsoft package repository before installing .NET Core 6:

```bash
sudo rpm -Uvh https://packages.microsoft.com/config/centos/7/packages-microsoft-prod.rpm
```

In the Cloud9 commandline install .net core 6.0 and .net core 3.1 using the following commands:

```bash
sudo yum install dotnet-sdk-6.0 -y 
sudo yum install aspnetcore-runtime-6.0 -y
sudo yum install dotnet-runtime-6.0 -y

sudo yum install dotnet-sdk-3.1 -y
sudo yum install aspnetcore-runtime-3.1 -y
sudo yum install dotnet-runtime-3.1 -y
dotnet --info
```

We will use .net core 6.0 for our applications later in the workshop. .net core 3.1 is needed by Terraform.

### Install Terraform 

We download the latest version of Terraform and install it using the following command lines:

```
wget https://releases.hashicorp.com/terraform/1.1.9/terraform_1.1.9_linux_amd64.zip
unzip terraform_1.1.9_linux_amd64.zip
sudo mv terraform /usr/local/bin
```

Now login to terraform in the Cloud9 environment using the following command line:

```
terraform login
```

Answer yes to the first question and then get your Terraform API token by opening the following URL in your browser:
https://app.terraform.io/app/settings/tokens?source=terraform-login
(If you do not have a Terraform account login/password then first create a free account following the instructions in the browser)

Copy the API token from the browser window and paste it into the Cloud9 environment commandline where the previous Terraform login command should be awaiting that token. You will not see the token text on the screen so just hit enter and if the token is accepted you should see a “Welcome to Terraform Cloud!” message.

### Install CDK for Terraform and deploy some infrastructure to AWS

Install CDK for Terraform by executing the following command line:

```
npm install --global cdktf-cli@latest
```

Check that the cdktf tool is correctly installed by running the following command line which shows various cdktf command line options:

```
cdktf help
```

If you see a list of cdktf command options after running cdktf help then you should be all good to go with the examples.

---

# Workshop Examples

## VPC with EC2 instance Terraform for CDK example

In the [first example](/vpc-ec2-example/README.md) you will deploy a VPC network 

## Serverless with Lambda functions and API Gateway

In the [second example](/lambda-example/README.md) you will deploy two serverless stacks based on Lambda and API Gateway

## Build your own architecture

If you are done with the first and second example you can either join one of the other groups to learn about AWS native CDK or you can try to convert an architecture your worked on into a CDK for Terraform application.
