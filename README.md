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

Go to the Cloud9 Environments page in the AWS Console [here](https://eu-north-1.console.aws.amazon.com/cloud9/home){:target="\_blank} 
Select the environment you want to resize and click "View details" button.
EC2 Instance >> Go To Instance
Select the instance and go to the "Storage" tab. And then click the Volume ID.
Select the Volume and click "Actions" button on the top right side. Then select "Modify volume".
Resize the volume to 50 GB. Then refresh the "Volumes" page.
After you complete modifying process, go to the "Instances" page again and Reboot the instance.
Wait about two minutes and reopen the Cloud9 environment.


In the Cloud9 environment commandline run df -h to verify the new 50 GB disk size.
