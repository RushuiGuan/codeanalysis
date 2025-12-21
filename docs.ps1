$working = $PSScriptRoot;

Get-ChildItem $working/docs | Remove-Item -Recurse -Force

xmldoc2md $working/Albatross.CodeAnalysis/bin/release/netstandard2.0/Albatross.CodeAnalysis.dll `
	-o $working/docs/ `
	--github-pages `
	--structure tree `
	--back-button

xmldoc2md $working/Albatross.CodeAnalysis.Polyfill/bin/release/netstandard2.0/Albatross.CodeAnalysis.Utility.dll `
	-o $working/docs/ `
	--github-pages `
	--structure tree `
	--back-button `
	--index-page-name $working/docs/index2

Get-Content $working/docs/index2.md >> $PSScriptRoot/docs/index.md

Remove-Item $working/docs/index2.md